namespace LawOffice.AI;

/// <summary>
/// Per-model token pricing, used to turn token usage into an estimated USD cost on every call.
/// Thinking in tokens/cost (not "requests"). Rates are in USD per 1,000 tokens and are
/// configurable via the "AiSettings:Cost" section so they can be kept current without a code change.
/// </summary>
public sealed class AiCostSettings
{
    public const string SectionName = "AiSettings:Cost";

    /// <summary>Per-model rates keyed by deployment/model name (case-insensitive).</summary>
    public Dictionary<string, TokenRate> Models { get; set; } =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Public Azure OpenAI list price for gpt-4.1-mini at time of writing; adjust as needed.
            ["gpt-4.1-mini"] = new TokenRate { InputPer1K = 0.0004m, OutputPer1K = 0.0016m },
        };

    /// <summary>
    /// Estimates the USD cost of a call. Returns null when the model has no configured rate, so the
    /// caller can log "cost: unknown" rather than a misleading $0.
    /// </summary>
    public decimal? Estimate(string? model, long inputTokens, long outputTokens)
    {
        if (string.IsNullOrWhiteSpace(model) || !Models.TryGetValue(model, out TokenRate? rate))
        {
            return null;
        }

        return (inputTokens / 1000m * rate.InputPer1K)
             + (outputTokens / 1000m * rate.OutputPer1K);
    }
}

/// <summary>USD price per 1,000 tokens, split by input (prompt) and output (completion).</summary>
public sealed class TokenRate
{
    public decimal InputPer1K { get; set; }
    public decimal OutputPer1K { get; set; }
}
