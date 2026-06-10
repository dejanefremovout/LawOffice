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

/// <summary>
/// Deterministic embedding generator for tests: maps text to a small bag-of-tokens vector via a stable
/// FNV-1a hash, so shared words land in shared buckets and semantically-overlapping text scores closer.
/// No network, fully reproducible within a process run.
/// </summary>
internal sealed class FakeEmbeddingGenerator(int dimensions = 32) : IEmbeddingGenerator<string, Embedding<float>>
{
    public Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
        IEnumerable<string> values,
        EmbeddingGenerationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        GeneratedEmbeddings<Embedding<float>> embeddings = new(values.Select(v => new Embedding<float>(Embed(v))));
        return Task.FromResult(embeddings);
    }

    public object? GetService(Type serviceType, object? serviceKey = null) => null;

    public void Dispose()
    {
    }

    private float[] Embed(string text)
    {
        float[] vector = new float[dimensions];
        foreach (string token in text.ToLowerInvariant().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
        {
            vector[StableHash(token) % (uint)dimensions] += 1f;
        }

        return vector;
    }

    private static uint StableHash(string value)
    {
        uint hash = 2166136261;
        foreach (char c in value)
        {
            hash = (hash ^ c) * 16777619;
        }

        return hash;
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
