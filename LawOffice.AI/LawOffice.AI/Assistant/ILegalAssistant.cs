namespace LawOffice.AI.Assistant;

/// <summary>
/// Answers a question grounded strictly in supplied context, returning a contract-validated
/// <see cref="LegalAnswer"/>. The assistant never throws on a bad model response: a violation that
/// cannot be repaired becomes a safe refusal.
/// </summary>
public interface ILegalAssistant
{
    /// <summary>
    /// Answers <paramref name="question"/> using only <paramref name="context"/>.
    /// </summary>
    /// <param name="question">The user's question (treated as untrusted data).</param>
    /// <param name="context">The sources the answer may be grounded in; citations must reference these.</param>
    /// <param name="promptVersion">
    /// Which registered prompt version to use; falls back to the configured default when null.
    /// Supplying this is how the same task is run two ways (e.g. terse vs few-shot).
    /// </param>
    Task<LegalAnswer> AnswerAsync(
        string question,
        IReadOnlyList<ContextSource> context,
        string? promptVersion = null,
        CancellationToken cancellationToken = default);
}
