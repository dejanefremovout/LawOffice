namespace LawOffice.AI.Assistant;

/// <summary>
/// A single unit of grounding context supplied to the assistant (e.g. a document chunk). The
/// <see cref="Id"/> is what the model cites and what the grounding check validates citations against.
/// These are now passed in by the caller; plan is that they come from tenant-scoped retrieval.
/// </summary>
/// <param name="Id">Stable identifier the model uses to cite this source.</param>
/// <param name="Text">The source text the model may answer from.</param>
public sealed record ContextSource(string Id, string Text);
