namespace LawOffice.AI.Assistant;

/// <summary>The outcome of validating a <see cref="LegalAnswer"/> against the output contract.</summary>
/// <param name="IsValid">True when the answer satisfies every contract rule.</param>
/// <param name="Violations">Human-readable reasons the answer failed; empty when valid.</param>
public sealed record ValidationResult(bool IsValid, IReadOnlyList<string> Violations)
{
    public static ValidationResult Valid { get; } = new(true, []);

    public static ValidationResult Invalid(IReadOnlyList<string> violations) => new(false, violations);
}

/// <summary>
/// Validates a <see cref="LegalAnswer"/> against the strict output contract. This is the
/// service-boundary check that makes structured output trustworthy: the schema gets it into the right
/// <em>shape</em>; the validator enforces the <em>semantics</em> (grounding, refusal consistency).
/// </summary>
public sealed class LegalAnswerValidator
{
    /// <summary>
    /// Checks an answer against the contract, given the context that was actually supplied to the model.
    /// </summary>
    public ValidationResult Validate(LegalAnswer answer, IEnumerable<ContextSource> context)
    {
        ArgumentNullException.ThrowIfNull(answer);
        ArgumentNullException.ThrowIfNull(context);

        HashSet<string> contextIds = context
            .Select(c => c.Id)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        List<string> violations = [];

        if (answer.IsRefusal)
        {
            // A refusal must be a clean refusal — no smuggled answer or citations alongside the reason.
            if (!string.IsNullOrWhiteSpace(answer.Answer))
            {
                violations.Add("A refusal must have an empty 'answer'.");
            }

            if (answer.Citations.Count > 0)
            {
                violations.Add("A refusal must have no citations.");
            }

            return violations.Count == 0 ? ValidationResult.Valid : ValidationResult.Invalid(violations);
        }

        // Not a refusal: it must be a real, grounded answer.
        if (string.IsNullOrWhiteSpace(answer.Answer))
        {
            violations.Add("An answer must be non-empty (or set 'refusedReason' to refuse).");
        }

        if (answer.Citations.Count == 0)
        {
            violations.Add("An answer must cite at least one supplied source.");
        }

        foreach (Citation citation in answer.Citations)
        {
            if (string.IsNullOrWhiteSpace(citation.SourceId) || !contextIds.Contains(citation.SourceId))
            {
                violations.Add(
                    $"Citation 'sourceId={citation.SourceId}' is not one of the supplied context sources.");
            }
        }

        return violations.Count == 0 ? ValidationResult.Valid : ValidationResult.Invalid(violations);
    }
}
