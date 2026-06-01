using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace LawOffice.AI.Resilience;

/// <summary>
/// Wraps an <see cref="IChatClient"/> with a Polly v8 resilience pipeline, treating the model as the
/// "flaky, slow, expensive external dependency" it is: per-attempt timeout, retry-with-backoff, and a
/// circuit breaker. This is the provider-agnostic resilience layer — it knows nothing about Azure.
/// </summary>
/// <remarks>
/// Strategy order (outer → inner) mirrors the Microsoft standard resilience handler:
/// retry → circuit breaker → per-attempt timeout. Retry sits outside the breaker so a tripped breaker
/// short-circuits fast; the timeout is innermost so it bounds each individual attempt.
/// </remarks>
public sealed class ResilientChatClient : DelegatingChatClient
{
    private readonly ResiliencePipeline<ChatResponse> _pipeline;
    private readonly ILogger<ResilientChatClient> _logger;

    public ResilientChatClient(
        IChatClient innerClient,
        AiResilienceOptions options,
        ILogger<ResilientChatClient> logger)
        : base(innerClient)
    {
        _logger = logger;
        _pipeline = BuildPipeline(options, logger);
    }

    public override Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return _pipeline
            .ExecuteAsync(
                async ct => await base.GetResponseAsync(messages, options, ct).ConfigureAwait(false),
                cancellationToken)
            .AsTask();
    }

    /// <summary>
    /// Streaming is forwarded to the inner client WITHOUT retry/circuit-breaker. A streamed response is
    /// stateful and partially consumed by the time a fault surfaces, so a transparent retry would
    /// duplicate or corrupt output. Design note: resilient streaming requires
    /// restart-from-scratch or buffering semantics, not a drop-in retry — so the caller owns recovery.
    /// </summary>
    public override IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return base.GetStreamingResponseAsync(messages, options, cancellationToken);
    }

    private static ResiliencePipeline<ChatResponse> BuildPipeline(
        AiResilienceOptions options,
        ILogger logger)
    {
        // Retry/breaker handle transient faults, but never the caller's cancellation, argument errors,
        // or an already-open circuit.
        static bool IsTransient(Exception? ex) =>
            ex is not null
            && ex is not OperationCanceledException
            && ex is not ArgumentException;

        return new ResiliencePipelineBuilder<ChatResponse>()
            // Outermost: retry the whole attempt (including a per-attempt timeout) with backoff + jitter.
            .AddRetry(new Polly.Retry.RetryStrategyOptions<ChatResponse>
            {
                MaxRetryAttempts = options.MaxRetryAttempts,
                Delay = options.BaseRetryDelay,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = args => ValueTask.FromResult(
                    IsTransient(args.Outcome.Exception)
                    && args.Outcome.Exception is not BrokenCircuitException),
                OnRetry = args =>
                {
                    logger.LogWarning(
                        args.Outcome.Exception,
                        "AI call failed (attempt {Attempt}); retrying in {Delay}.",
                        args.AttemptNumber + 1,
                        args.RetryDelay);
                    return ValueTask.CompletedTask;
                },
            })
            // Middle: trip after too many failures so we stop hammering a downed endpoint.
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<ChatResponse>
            {
                FailureRatio = options.CircuitBreakerFailureRatio,
                MinimumThroughput = options.CircuitBreakerMinimumThroughput,
                SamplingDuration = options.CircuitBreakerSamplingDuration,
                BreakDuration = options.CircuitBreakerBreakDuration,
                ShouldHandle = args => ValueTask.FromResult(IsTransient(args.Outcome.Exception)),
                OnOpened = args =>
                {
                    logger.LogError(
                        args.Outcome.Exception,
                        "AI circuit breaker OPENED for {BreakDuration}.",
                        args.BreakDuration);
                    return ValueTask.CompletedTask;
                },
                OnClosed = _ =>
                {
                    logger.LogInformation("AI circuit breaker closed; calls flowing again.");
                    return ValueTask.CompletedTask;
                },
            })
            // Innermost: bound each individual attempt. Surfaces as TimeoutRejectedException, which the
            // retry layer treats as transient.
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = options.AttemptTimeout,
            })
            .Build();
    }
}
