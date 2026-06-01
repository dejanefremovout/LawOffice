using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace LawOffice.AI.Tests;

/// <summary>
/// Stateful fake chat client: the handler decides per-attempt whether to throw or return, and the
/// fake counts calls so tests can assert how many attempts the resilience layer made.
/// </summary>
internal sealed class FakeChatClient(Func<int, ChatResponse> handler) : IChatClient
{
    public int CallCount { get; private set; }

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        CallCount++;
        return Task.FromResult(handler(CallCount));
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public object? GetService(Type serviceType, object? serviceKey = null) => null;

    public void Dispose()
    {
    }
}

/// <summary>Captures formatted log messages so tests can assert on emitted observability output.</summary>
internal sealed class ListLogger<T> : ILogger<T>
{
    public List<string> Messages { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter) =>
        Messages.Add(formatter(state, exception));
}
