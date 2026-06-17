using LawOffice.AI.Retrieval;

namespace LawOffice.AI.Tests.Retrieval;

/// <summary>
/// The headline Day-3 artifact: proves that a query scoped to one tenant can never retrieve another
/// tenant's vectors — even when the other tenant holds the geometrically closest chunk. In a regulated
/// legal-tech product this is the difference between "isolated" and a privilege-breaching data leak
/// (OWASP LLM "Vector and Embedding Weaknesses").
/// </summary>
public class TenantIsolationTests
{
    private static VectorRecord Record(string officeId, string docId, string section, float[] embedding) =>
        new()
        {
            Id = $"{officeId}:{docId}:{section}",
            OfficeId = officeId,
            DocId = docId,
            Section = section,
            DocType = "lease",
            Text = $"{docId} {section}",
            Embedding = embedding,
        };

    [Fact]
    public async Task Search_returns_only_the_querying_tenants_chunks_even_when_another_tenant_is_closer()
    {
        InMemoryVectorStore store = new();
        float[] query = [1f, 0f, 0f];

        // Office B owns the single closest vector to the query — an exact match.
        await store.UpsertAsync("office-b", [Record("office-b", "privileged-b", "secret", [1f, 0f, 0f])]);

        // Office A's nearest chunk is deliberately further away.
        await store.UpsertAsync("office-a",
        [
            Record("office-a", "lease-12", "termination", [0.8f, 0.2f, 0f]),
            Record("office-a", "lease-12", "rent", [0f, 1f, 0f]),
        ]);

        IReadOnlyList<ScoredChunk> hits = await store.SearchAsync("office-a", query, topK: 5);

        hits.ShouldNotBeEmpty();
        hits.ShouldAllBe(h => h.Record.OfficeId == "office-a");
        hits.Select(h => h.Record.Id).ShouldNotContain("office-b:privileged-b:secret");
    }

    [Fact]
    public async Task Search_for_a_tenant_with_no_data_returns_empty()
    {
        InMemoryVectorStore store = new();
        await store.UpsertAsync("office-a", [Record("office-a", "d", "s", [1f, 0f])]);

        IReadOnlyList<ScoredChunk> hits = await store.SearchAsync("office-z", new float[] { 1f, 0f }, topK: 5);

        hits.ShouldBeEmpty();
    }

    [Fact]
    public async Task Upsert_rejects_a_record_whose_office_does_not_match_the_scope()
    {
        InMemoryVectorStore store = new();

        await Should.ThrowAsync<ArgumentException>(() =>
            store.UpsertAsync("office-a", [Record("office-b", "d", "s", [1f, 0f])]));
    }
}
