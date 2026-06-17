using LawOffice.AI.Assistant;
using LawOffice.AI.Retrieval;
using Microsoft.Extensions.Logging.Abstractions;

namespace LawOffice.AI.Tests.Retrieval;

public class RagPipelineTests
{
    // Answers (citing the first source) whenever any context is present; refuses on empty context.
    private static LegalAnswer AnswerIfGrounded(string _, IReadOnlyList<ContextSource> context) =>
        context.Count == 0
            ? LegalAnswer.Refusal("No context to ground an answer in.")
            : new LegalAnswer
            {
                Answer = "answer",
                Citations = [new Citation(context[0].Id)],
                Confidence = Confidence.High,
            };

    private static (RagPipeline Pipeline, TenantDocumentRetriever Retriever, StubLegalAssistant Assistant) Build(
        RetrievalSettings settings,
        Func<string, IReadOnlyList<ContextSource>, LegalAnswer>? handler = null)
    {
        TenantDocumentRetriever retriever = new(new FakeEmbeddingGenerator(), new InMemoryVectorStore());
        StubLegalAssistant assistant = new(handler ?? AnswerIfGrounded);
        RagPipeline pipeline = new(retriever, new LexicalReranker(), assistant, settings, NullLogger<RagPipeline>.Instance);
        return (pipeline, retriever, assistant);
    }

    [Fact]
    public async Task Retrieves_broad_pool_then_narrows_to_topK_and_grounds_the_assistant_on_those()
    {
        RetrievalSettings settings = new() { CandidatePoolSize = 20, TopK = 2 };
        (RagPipeline pipeline, TenantDocumentRetriever retriever, StubLegalAssistant assistant) = Build(settings);
        await retriever.IngestAsync("office-a",
        [
            new("lease", "Termination", 0, "the tenant shall give sixty days termination notice", "lease"),
            new("lease", "Rent", 0, "monthly rent is due on the first day", "lease"),
            new("lease", "Deposit", 0, "a security deposit is held in escrow", "lease"),
            new("lease", "Repairs", 0, "the landlord maintains the structure", "lease"),
        ]);

        RagResult result = await pipeline.AskAsync("office-a", "termination notice period");

        result.Retrieved.Count.ShouldBe(4);              // broad recall over the whole tenant set
        result.Reranked.Count.ShouldBe(2);               // narrowed to TopK
        result.Answer.IsRefusal.ShouldBeFalse();
        assistant.LastContext!.Count.ShouldBe(result.Reranked.Count);
    }

    [Fact]
    public async Task Is_tenant_scoped_so_another_tenants_chunks_are_never_retrieved()
    {
        RetrievalSettings settings = new() { CandidatePoolSize = 20, TopK = 5 };
        (RagPipeline pipeline, TenantDocumentRetriever retriever, _) = Build(settings);
        await retriever.IngestAsync("office-a", [new("a", "S", 0, "alpha beta gamma", "t")]);
        await retriever.IngestAsync("office-b", [new("b", "S", 0, "alpha beta gamma", "t")]);

        RagResult result = await pipeline.AskAsync("office-a", "alpha beta gamma");

        result.Retrieved.ShouldAllBe(c => c.DocId == "a");
        result.Reranked.ShouldNotContain(c => c.DocId == "b");
    }

    [Fact]
    public async Task Refuses_when_the_tenant_has_no_matching_documents()
    {
        RetrievalSettings settings = new() { CandidatePoolSize = 20, TopK = 5 };
        (RagPipeline pipeline, _, _) = Build(settings);

        RagResult result = await pipeline.AskAsync("office-empty", "anything at all");

        result.Retrieved.ShouldBeEmpty();
        result.Reranked.ShouldBeEmpty();
        result.Answer.IsRefusal.ShouldBeTrue();
    }
}
