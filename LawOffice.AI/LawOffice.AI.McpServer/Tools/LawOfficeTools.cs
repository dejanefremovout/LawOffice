using System.ComponentModel;
using LawOffice.AI.Retrieval;
using ModelContextProtocol.Server;

namespace LawOffice.AI.McpServer.Tools;

/// <summary>
/// The LawOffice capabilities exposed to MCP hosts as tools. Two deliberately different shapes:
/// <list type="bullet">
/// <item><c>SearchDocuments</c> — unstructured knowledge via RAG retrieval.</item>
/// <item><c>GetCaseStatus</c> — structured truth via a system-of-record lookup.</item>
/// </list>
/// Security spine: neither tool takes a tenant/office parameter. Each resolves its office from the
/// injected <see cref="ITenantContext"/> (the session's authenticated identity), so the model has no slot
/// to name a tenant and cannot reach another office's data.
/// Tools are read-only and least-privilege.
/// </summary>
[McpServerToolType]
public sealed class LawOfficeTools
{
    [McpServerTool(Name = "SearchDocuments")]
    [Description(
        "Searches the current law office's legal documents for passages relevant to the query and returns " +
        "the most relevant chunks (with document id, section, and a snippet) to use as grounding. Use this " +
        "for questions about the content of contracts, agreements, and other unstructured documents.")]
    public static async Task<IReadOnlyList<DocumentHit>> SearchDocuments(
        TenantDocumentRetriever retriever,
        ITenantContext tenant,
        RetrievalSettings settings,
        [Description("The natural-language search query.")] string query,
        CancellationToken cancellationToken)
    {
        // Tenant comes from the authenticated session, never from the model.
        IReadOnlyList<RetrievedCandidate> candidates = await retriever
            .RetrieveCandidatesAsync(tenant.OfficeId, query, settings.TopK, cancellationToken)
            .ConfigureAwait(false);

        return candidates
            .Select(c => new DocumentHit(c.DocId, c.Section, c.Text, c.VectorScore))
            .ToList();
    }

    [McpServerTool(Name = "GetCaseStatus")]
    [Description(
        "Returns the authoritative status of a case in the current law office, looked up by case id. Use " +
        "this for the live status of a specific case rather than searching documents.")]
    public static async Task<CaseStatusResult> GetCaseStatus(
        ICaseStatusProvider cases,
        ITenantContext tenant,
        [Description("The case id, e.g. 'CASE-001'.")] string caseId,
        CancellationToken cancellationToken)
    {
        string? status = await cases
            .GetStatusAsync(tenant.OfficeId, caseId, cancellationToken)
            .ConfigureAwait(false);

        return new CaseStatusResult(caseId, status is not null, status);
    }
}
