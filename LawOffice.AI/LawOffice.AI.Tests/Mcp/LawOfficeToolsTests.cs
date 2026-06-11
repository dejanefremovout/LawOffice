using LawOffice.AI.McpServer;
using LawOffice.AI.McpServer.Tools;
using LawOffice.AI.Retrieval;

namespace LawOffice.AI.Tests.Mcp;

/// <summary>
/// Proves the Day-5 security spine in code: the MCP tools take no tenant parameter and scope every call
/// to the session's <see cref="ITenantContext"/>. Even when another office holds identical (or closer)
/// content, a tool call in office-a's session can only ever return office-a's data — the model has no
/// slot to name a different tenant.
/// </summary>
public class LawOfficeToolsTests
{
    private static DocumentChunk Chunk(string docId, string section, string text, string docType = "lease") =>
        new(docId, section, 0, text, docType);

    private static async Task<TenantDocumentRetriever> SeedTwoTenantsAsync()
    {
        TenantDocumentRetriever retriever = new(new FakeEmbeddingGenerator(), new InMemoryVectorStore());

        await retriever.IngestAsync("office-a",
        [
            Chunk("lease-12", "5. Termination",
                "The tenant shall give sixty days written notice before terminating this lease."),
        ]);

        // Office B holds an identical passage under a privileged doc id — the leak we must prevent.
        await retriever.IngestAsync("office-b",
        [
            Chunk("privileged-b", "secret",
                "The tenant shall give sixty days written notice before terminating this lease."),
        ]);

        return retriever;
    }

    [Fact]
    public async Task SearchDocuments_returns_only_the_session_offices_documents()
    {
        TenantDocumentRetriever retriever = await SeedTwoTenantsAsync();
        ITenantContext tenant = new ConfiguredTenantContext("office-a");
        RetrievalSettings settings = new() { TopK = 5 };

        IReadOnlyList<DocumentHit> hits = await LawOfficeTools.SearchDocuments(
            retriever, tenant, settings, "termination notice period", CancellationToken.None);

        hits.ShouldNotBeEmpty();
        hits.ShouldAllBe(h => h.DocId == "lease-12");
        hits.Select(h => h.DocId).ShouldNotContain("privileged-b");
    }

    [Fact]
    public async Task SearchDocuments_maps_candidates_to_document_hits()
    {
        TenantDocumentRetriever retriever = await SeedTwoTenantsAsync();
        ITenantContext tenant = new ConfiguredTenantContext("office-a");
        RetrievalSettings settings = new() { TopK = 5 };

        IReadOnlyList<DocumentHit> hits = await LawOfficeTools.SearchDocuments(
            retriever, tenant, settings, "termination notice period", CancellationToken.None);

        DocumentHit hit = hits.ShouldHaveSingleItem();
        hit.DocId.ShouldBe("lease-12");
        hit.Section.ShouldBe("5. Termination");
        hit.Snippet.ShouldContain("sixty days");
    }

    [Fact]
    public async Task GetCaseStatus_returns_status_for_a_case_in_the_session_office()
    {
        ICaseStatusProvider provider = InMemoryCaseStatusProvider.WithDemoSeed();
        ITenantContext tenant = new ConfiguredTenantContext("office-a");

        CaseStatusResult result = await LawOfficeTools.GetCaseStatus(
            provider, tenant, "CASE-001", CancellationToken.None);

        result.Found.ShouldBeTrue();
        result.Status.ShouldBe("Active");
        result.CaseId.ShouldBe("CASE-001");
    }

    [Fact]
    public async Task GetCaseStatus_does_not_resolve_a_case_belonging_to_another_office()
    {
        InMemoryCaseStatusProvider provider = new();
        provider.Seed("office-b", "CASE-777", "Active");
        ITenantContext tenant = new ConfiguredTenantContext("office-a");

        CaseStatusResult result = await LawOfficeTools.GetCaseStatus(
            provider, tenant, "CASE-777", CancellationToken.None);

        result.Found.ShouldBeFalse();
        result.Status.ShouldBeNull();
    }
}
