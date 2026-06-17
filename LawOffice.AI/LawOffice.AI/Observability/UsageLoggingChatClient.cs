using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace LawOffice.AI.Observability;

/// <summary>
/// Middleware that measures every model call — tokens in/out, latency, model version, estimated cost —
/// and emits it as a structured log line. Sits below the resilience layer so it only meters calls that
/// actually reached the model (a retried-then-succeeded call is logged once, on success).
/// </summary>
public sealed class UsageLoggingChatClient : DelegatingChatClient
{
    private readonly AiCostSettings _cost;
    private readonly ILogger<UsageLoggingChatClient> _logger;

    public UsageLoggingChatClient(
        IChatClient innerClient,
        AiCostSettings cost,
        ILogger<UsageLoggingChatClient> logger)
        : base(innerClient)
    {
        _cost = cost;
        _logger = logger;
    }

    public override async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        long start = Stopwatch.GetTimestamp();
        ChatResponse response = await base.GetResponseAsync(messages, options, cancellationToken)
            .ConfigureAwait(false);

        AiCallMetrics metrics = BuildMetrics(
            model: response.ModelId,
            usage: response.Usage,
            latency: Stopwatch.GetElapsedTime(start),
            streaming: false);

        Log(metrics);
        return response;
    }

    public override async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        long start = Stopwatch.GetTimestamp();
        string? model = null;
        UsageDetails accumulated = new();

        await foreach (ChatResponseUpdate update in
            base.GetStreamingResponseAsync(messages, options, cancellationToken).ConfigureAwait(false))
        {
            model ??= update.ModelId;

            // Usage arrives as a dedicated content part, typically on the final update(s).
            foreach (AIContent content in update.Contents)
            {
                if (content is UsageContent usage)
                {
                    accumulated.Add(usage.Details);
                }
            }

            yield return update;
        }

        AiCallMetrics metrics = BuildMetrics(
            model: model,
            usage: accumulated,
            latency: Stopwatch.GetElapsedTime(start),
            streaming: true);

        Log(metrics);
    }

    private AiCallMetrics BuildMetrics(string? model, UsageDetails? usage, TimeSpan latency, bool streaming)
    {
        long inputTokens = usage?.InputTokenCount ?? 0;
        long outputTokens = usage?.OutputTokenCount ?? 0;

        return new AiCallMetrics(
            Model: model,
            InputTokens: inputTokens,
            OutputTokens: outputTokens,
            Latency: latency,
            EstimatedCostUsd: _cost.Estimate(model, inputTokens, outputTokens),
            Streaming: streaming);
    }

    private void Log(AiCallMetrics m)
    {
        _logger.LogInformation(
            "AI call: model={Model} streaming={Streaming} inputTokens={InputTokens} outputTokens={OutputTokens} totalTokens={TotalTokens} latencyMs={LatencyMs} estimatedCostUsd={EstimatedCostUsd}",
            m.Model ?? "unknown",
            m.Streaming,
            m.InputTokens,
            m.OutputTokens,
            m.TotalTokens,
            (long)m.Latency.TotalMilliseconds,
            m.EstimatedCostUsd?.ToString("0.######") ?? "unknown");
    }
}
