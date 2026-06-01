namespace LawOffice.AI.Resilience;

/// <summary>
/// Tunables for the resilience pipeline that wraps the model call. Defaults treat the LLM endpoint
/// as a slow, flaky external dependency: a generous per-attempt timeout, a few retries with
/// exponential backoff + jitter, and a circuit breaker to stop hammering a failing service.
/// Bound from the "AiSettings:Resilience" section.
/// </summary>
public sealed class AiResilienceOptions
{
    public const string SectionName = "AiSettings:Resilience";

    /// <summary>Per-attempt timeout. The harness "chaos" mode sets this to 1s to force failures.</summary>
    public TimeSpan AttemptTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>Number of retry attempts after the first try (so total tries = MaxRetryAttempts + 1).</summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>Base delay for exponential backoff between retries.</summary>
    public TimeSpan BaseRetryDelay { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>Failure ratio (0..1) within the sampling window that trips the circuit breaker.</summary>
    public double CircuitBreakerFailureRatio { get; set; } = 0.5;

    /// <summary>Minimum number of calls in the sampling window before the breaker can trip.</summary>
    public int CircuitBreakerMinimumThroughput { get; set; } = 5;

    /// <summary>Rolling window over which the failure ratio is measured.</summary>
    public TimeSpan CircuitBreakerSamplingDuration { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>How long the circuit stays open before allowing a trial call.</summary>
    public TimeSpan CircuitBreakerBreakDuration { get; set; } = TimeSpan.FromSeconds(15);
}
