using System.ComponentModel;

namespace LawOffice.AI.McpServer.Tools;

/// <summary>
/// A single document chunk returned by <c>SearchDocuments</c>. A stable, tool-facing DTO kept separate
/// from the internal <c>RetrievedCandidate</c> so the tool's wire contract doesn't leak retrieval
/// internals and can evolve independently.
/// </summary>
/// <param name="DocId">Source document id.</param>
/// <param name="Section">Clause/section label the snippet came from.</param>
/// <param name="Snippet">The chunk text — the grounding the host's model should cite.</param>
/// <param name="Score">Vector similarity score (higher = more similar).</param>
public sealed record DocumentHit(
    [property: Description("Source document id.")] string DocId,
    [property: Description("Clause or section label within the document.")] string Section,
    [property: Description("The matching text, to be cited as grounding.")] string Snippet,
    [property: Description("Relevance score; higher is more similar.")] double Score);

/// <summary>Result of <c>GetCaseStatus</c>: the case's status, or <see cref="Found"/> = false if no such
/// case exists for the session's office.</summary>
/// <param name="CaseId">The case id that was looked up.</param>
/// <param name="Found">Whether a case with that id exists for the current office.</param>
/// <param name="Status">The case status when found; otherwise <see langword="null"/>.</param>
public sealed record CaseStatusResult(
    [property: Description("The case id that was looked up.")] string CaseId,
    [property: Description("True if a case with that id exists for this office.")] bool Found,
    [property: Description("The case status when found; null otherwise.")] string? Status);
