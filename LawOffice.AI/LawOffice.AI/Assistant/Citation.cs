namespace LawOffice.AI.Assistant;

/// <summary>
/// A reference from an answer back to the supplied context that supports it. <see cref="SourceId"/>
/// must match the id of a <see cref="ContextSource"/> that was provided to the model — the grounding
/// check rejects citations to ids that were never supplied.
/// </summary>
/// <param name="SourceId">Id of the supporting <see cref="ContextSource"/>.</param>
/// <param name="Quote">Optional verbatim snippet from that source backing the claim.</param>
public sealed record Citation(string SourceId, string? Quote = null);
