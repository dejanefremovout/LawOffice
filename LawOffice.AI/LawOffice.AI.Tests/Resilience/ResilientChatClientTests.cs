using LawOffice.AI.Resilience;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;
using Polly.CircuitBreaker;

namespace LawOffice.AI.Tests.Resilience;

public class ResilientChatClientTests
{
    private static readonly List<ChatMessage> Messages =
        [new(ChatRole.User, "hello")];

    [Fact]
    public async Task Retries_transient_failures_then_returns_success()
    {
        ChatResponse success = new(new ChatMessage(ChatRole.Assistant, "ok"));
        FakeChatClient inner = new(attempt =>
            attempt <= 2 ? throw new HttpRequestException("transient") : success);

        AiResilienceOptions options = new()
        {
            MaxRetryAttempts = 3,
            BaseRetryDelay = TimeSpan.FromMilliseconds(1),
            AttemptTimeout = TimeSpan.FromSeconds(10),
            CircuitBreakerMinimumThroughput = 100, // don't trip the breaker during this test
        };

        ResilientChatClient sut = new(inner, options, NullLogger<ResilientChatClient>.Instance);

        ChatResponse result = await sut.GetResponseAsync(Messages);

        result.ShouldBe(success);
        inner.CallCount.ShouldBe(3); // 2 failures + 1 success
    }

    [Fact]
    public async Task Opens_circuit_when_failures_persist_and_stops_hammering()
    {
        FakeChatClient inner = new(_ => throw new HttpRequestException("down"));

        AiResilienceOptions options = new()
        {
            MaxRetryAttempts = 50,
            BaseRetryDelay = TimeSpan.FromMilliseconds(1),
            AttemptTimeout = TimeSpan.FromSeconds(10),
            CircuitBreakerFailureRatio = 0.5,
            CircuitBreakerMinimumThroughput = 3,
            CircuitBreakerSamplingDuration = TimeSpan.FromSeconds(30),
            CircuitBreakerBreakDuration = TimeSpan.FromSeconds(30),
        };

        ResilientChatClient sut = new(inner, options, NullLogger<ResilientChatClient>.Instance);

        // Once the breaker opens it short-circuits with BrokenCircuitException, which the retry layer
        // does not treat as transient -> the call fails fast instead of exhausting all 50 retries.
        await Should.ThrowAsync<BrokenCircuitException>(() => sut.GetResponseAsync(Messages));
        inner.CallCount.ShouldBeLessThan(options.MaxRetryAttempts + 1);
    }

    [Fact]
    public async Task Does_not_retry_caller_cancellation()
    {
        FakeChatClient inner = new(_ => throw new OperationCanceledException());

        AiResilienceOptions options = new()
        {
            MaxRetryAttempts = 5,
            BaseRetryDelay = TimeSpan.FromMilliseconds(1),
            AttemptTimeout = TimeSpan.FromSeconds(10),
            CircuitBreakerMinimumThroughput = 100,
        };

        ResilientChatClient sut = new(inner, options, NullLogger<ResilientChatClient>.Instance);

        await Should.ThrowAsync<OperationCanceledException>(() => sut.GetResponseAsync(Messages));
        inner.CallCount.ShouldBe(1); // cancellation is not a transient fault
    }
}
