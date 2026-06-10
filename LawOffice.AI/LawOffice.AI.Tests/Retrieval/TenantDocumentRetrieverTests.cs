using LawOffice.AI.Assistant;
using LawOffice.AI.Retrieval;

namespace LawOffice.AI.Tests.Retrieval;

public class TenantDocumentRetrieverTests
{
    private static TenantDocumentRetriever NewRetriever() =>
        new(new FakeEmbeddingGenerator(), new InMemoryVectorStore());

    [Fact]
    public async Task Ingest_then_retrieve_returns_the_most_relevant_chunk_as_citation_ready_context()
    {
        TenantDocumentRetriever retriever = NewRetriever();
        List<DocumentChunk> chunks =
        [
            new("lease-12", "Termination", 0, "The tenant shall give sixty days notice to terminate the lease.", "lease"),
            new("lease-12", "Rent", 0, "Monthly rent is EUR 850 due on the first day.", "lease"),
        ];
        await retriever.IngestAsync("office-a", chunks);

        IReadOnlyList<ContextSource> context = await retriever.RetrieveAsync("office-a", "terminate lease notice", topK: 1);

        context.ShouldHaveSingleItem();
        context[0].Id.ShouldBe("lease-12#Termination");
    }

    [Fact]
    public async Task Retrieve_is_scoped_to_the_tenant()
    {
        TenantDocumentRetriever retriever = NewRetriever();
        await retriever.IngestAsync("office-a", [new DocumentChunk("d", "S", 0, "alpha beta gamma", "t")]);

        IReadOnlyList<ContextSource> context = await retriever.RetrieveAsync("office-b", "alpha beta gamma", topK: 5);

        context.ShouldBeEmpty();
    }
}
