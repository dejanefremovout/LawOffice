using LawOffice.AI.Retrieval;

namespace LawOffice.AI.Tests.Retrieval;

public class LexicalRerankerTests
{
    private static readonly LexicalReranker Reranker = new();

    [Fact]
    public void Promotes_a_lexical_match_that_pure_vector_ranking_would_bury()
    {
        // Pure vector order is A, B, C. Only B contains the query's exact terms, so the reranker must
        // pull it to the top — this is the hybrid-search win on identifiers/keywords cosine alone misses.
        List<RetrievedCandidate> candidates =
        [
            new("doc", "A", "general discussion of obligations and agreements", VectorScore: 1.0),
            new("doc", "B", "the termination notice period is sixty days", VectorScore: 0.8),
            new("doc", "C", "unrelated boilerplate about signatures", VectorScore: 0.2),
        ];

        IReadOnlyList<RetrievedCandidate> ranked = Reranker.Rerank("termination notice period", candidates, topK: 1);

        ranked.ShouldHaveSingleItem();
        ranked[0].Section.ShouldBe("B");
    }

    [Fact]
    public void Never_returns_more_than_topK_or_more_than_were_supplied()
    {
        List<RetrievedCandidate> candidates =
        [
            new("doc", "A", "alpha", 0.9),
            new("doc", "B", "beta", 0.5),
            new("doc", "C", "gamma", 0.1),
        ];

        Reranker.Rerank("alpha beta gamma", candidates, topK: 2).Count.ShouldBe(2);
        Reranker.Rerank("alpha beta gamma", candidates, topK: 10).Count.ShouldBe(3);
    }

    [Fact]
    public void Is_deterministic_for_the_same_input()
    {
        List<RetrievedCandidate> candidates =
        [
            new("doc", "A", "lease termination clause", 0.7),
            new("doc", "B", "rent payment schedule", 0.6),
            new("doc", "C", "termination notice requirement", 0.65),
        ];

        IReadOnlyList<RetrievedCandidate> first = Reranker.Rerank("termination notice", candidates, topK: 3);
        IReadOnlyList<RetrievedCandidate> second = Reranker.Rerank("termination notice", candidates, topK: 3);

        first.Select(c => c.Section).ShouldBe(second.Select(c => c.Section));
    }

    [Fact]
    public void Returns_empty_for_no_candidates_or_non_positive_topK()
    {
        Reranker.Rerank("anything", [], topK: 5).ShouldBeEmpty();
        Reranker.Rerank("anything", [new RetrievedCandidate("d", "s", "text", 0.5)], topK: 0).ShouldBeEmpty();
    }
}
