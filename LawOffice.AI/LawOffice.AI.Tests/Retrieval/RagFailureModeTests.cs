using LawOffice.AI.Assistant;
using LawOffice.AI.Retrieval;
using Microsoft.Extensions.Logging.Abstractions;

namespace LawOffice.AI.Tests.Retrieval;

/// <summary>
/// Encodes the two canonical RAG failure modes as regression tests: a too-narrow candidate pool that
/// excludes the answer-bearing chunk (retrieval miss), and chunking that splits one fact across two
/// chunks so neither is self-sufficient (chunk-boundary damage). Both reproduce the failure and show the
/// fix — widening the pool, and structure-aware chunking respectively.
/// </summary>
public class RagFailureModeTests
{
    private static RagPipeline Build(
        TenantDocumentRetriever retriever,
        RetrievalSettings settings,
        Func<string, IReadOnlyList<ContextSource>, LegalAnswer> handler) =>
        new(retriever, new LexicalReranker(), new StubLegalAssistant(handler), settings, NullLogger<RagPipeline>.Instance);

    // Answers only when an answer-bearing chunk (one containing the marker) reached the context.
    private static Func<string, IReadOnlyList<ContextSource>, LegalAnswer> AnswersOnlyWhenContains(string marker) =>
        (_, context) =>
        {
            ContextSource? bearing = context.FirstOrDefault(c => c.Text.Contains(marker, StringComparison.OrdinalIgnoreCase));
            return bearing is null
                ? LegalAnswer.Refusal("The answer is not supported by the retrieved context.")
                : new LegalAnswer { Answer = "answer", Citations = [new Citation(bearing.Id)], Confidence = Confidence.High };
        };

    [Fact]
    public async Task Retrieval_miss_a_pool_too_small_to_include_the_answer_chunk_forces_a_refusal()
    {
        TenantDocumentRetriever retriever = new(new FakeEmbeddingGenerator(), new InMemoryVectorStore());
        // Four distractors are an exact token match for the query (top vector hits); the lone answer
        // chunk ("epsilon") overlaps less, so it ranks last and falls outside a pool of one.
        await retriever.IngestAsync("office-a",
        [
            new("d", "D1", 0, "alpha beta gamma delta", "t"),
            new("d", "D2", 0, "alpha beta gamma delta", "t"),
            new("d", "D3", 0, "alpha beta gamma delta", "t"),
            new("d", "D4", 0, "alpha beta gamma delta", "t"),
            new("d", "ANS", 0, "alpha epsilon", "t"),
        ]);
        Func<string, IReadOnlyList<ContextSource>, LegalAnswer> handler = AnswersOnlyWhenContains("epsilon");

        RagResult missed = await Build(retriever, new RetrievalSettings { CandidatePoolSize = 1, TopK = 1 }, handler)
            .AskAsync("office-a", "alpha beta gamma delta");

        missed.Retrieved.Count.ShouldBe(1);
        missed.Answer.IsRefusal.ShouldBeTrue();

        // Fix: widen the pool so the answer chunk is retrieved, and the assistant can ground on it.
        RagResult recovered = await Build(retriever, new RetrievalSettings { CandidatePoolSize = 20, TopK = 5 }, handler)
            .AskAsync("office-a", "alpha beta gamma delta");

        recovered.Reranked.ShouldContain(c => c.Text.Contains("epsilon"));
        recovered.Answer.IsRefusal.ShouldBeFalse();
    }

    [Fact]
    public async Task Chunk_boundary_damage_a_fact_split_across_chunks_cannot_be_grounded_until_chunking_keeps_it_whole()
    {
        // One fact whose two key terms ("notice" ... "terminating") sit at opposite ends of the text.
        const string head = "this provision sets out the notice obligations of the tenant under the present lease agreement";
        string text = head + " terminating";
        Func<string, IReadOnlyList<ContextSource>, LegalAnswer> handler = (_, context) =>
        {
            ContextSource? whole = context.FirstOrDefault(c =>
                c.Text.Contains("notice", StringComparison.OrdinalIgnoreCase) &&
                c.Text.Contains("terminating", StringComparison.OrdinalIgnoreCase));
            return whole is null
                ? LegalAnswer.Refusal("The answer is split across chunks and not fully supported.")
                : new LegalAnswer { Answer = "answer", Citations = [new Citation(whole.Id)], Confidence = Confidence.High };
        };

        RetrievalSettings settings = new();
        TenantDocumentRetriever retriever = new(new FakeEmbeddingGenerator(), new InMemoryVectorStore());

        // Zero-overlap fixed chunking sized to split the fact: "notice" lands in one chunk, "terminating" the next.
        await retriever.IngestAsync("office-fixed",
            new FixedSizeChunker(windowChars: head.Length, overlapChars: 0).Chunk("doc", "lease", text));
        // Structure-aware chunking keeps the single passage intact.
        await retriever.IngestAsync("office-structured",
            new StructureAwareChunker().Chunk("doc", "lease", text));

        RagResult split = await Build(retriever, settings, handler).AskAsync("office-fixed", "notice terminating");
        split.Answer.IsRefusal.ShouldBeTrue();

        RagResult whole = await Build(retriever, settings, handler).AskAsync("office-structured", "notice terminating");
        whole.Answer.IsRefusal.ShouldBeFalse();
    }
}
