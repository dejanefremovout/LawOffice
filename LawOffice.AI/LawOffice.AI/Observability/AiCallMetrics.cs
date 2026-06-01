namespace LawOffice.AI.Observability;

/// <summary>
/// What we record for every model call. Thinking in tokens, latency, model version, and cost (rather than "requests").
/// </summary>
public sealed record AiCallMetrics(
    string? Model,
    long InputTokens,
    long OutputTokens,
    TimeSpan Latency,
    decimal? EstimatedCostUsd,
    bool Streaming)
{
    public long TotalTokens => InputTokens + OutputTokens;
}
