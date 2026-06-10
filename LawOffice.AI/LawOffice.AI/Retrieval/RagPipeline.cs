using LawOffice.AI.Assistant;
using Microsoft.Extensions.Logging;

namespace LawOffice.AI.Retrieval;

/// <summary>
/// Default <see cref="IRagPipeline"/>: retrieve a broad candidate pool (tenant-scoped) → rerank to a
/// tight top-K → answer grounded strictly in the kept context. The reranked context is presented to the
/// model best-at-the-edges to counter the "lost in the middle" attention bias.
/// </summary>
public sealed class RagPipeline : IRagPipeline
{
    private readonly TenantDocumentRetriever _retriever;
    private readonly IReranker _reranker;
    private readonly ILegalAssistant _assistant;
    private readonly RetrievalSettings _settings;
    private readonly ILogger<RagPipeline> _logger;

    public RagPipeline(
        TenantDocumentRetriever retriever,
        IReranker reranker,
        ILegalAssistant assistant,
        RetrievalSettings settings,
        ILogger<RagPipeline> logger)
    {
        _retriever = retriever;
        _reranker = reranker;
        _assistant = assistant;
        _settings = settings;
        _logger = logger;
    }

    public async Task<RagResult> AskAsync(
        string officeId,
        string question,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(question);

        // 1) Broad recall: cheap vector retrieval over a generous pool.
        IReadOnlyList<RetrievedCandidate> retrieved = await _retriever
            .RetrieveCandidatesAsync(officeId, question, _settings.CandidatePoolSize, cancellationToken)
            .ConfigureAwait(false);

        // 2) High precision: rerank and keep only the best top-K.
        IReadOnlyList<RetrievedCandidate> reranked = _reranker.Rerank(question, retrieved, _settings.TopK);

        // 3) Ground the answer in the kept context (best chunks bracketing the edges).
        IReadOnlyList<ContextSource> context = ToEdgeOrderedContext(reranked);
        LegalAnswer answer = await _assistant
            .AnswerAsync(question, context, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation(
            "RAG query for {OfficeId}: retrieved {Retrieved} -> reranked {Reranked}; refused={Refused}",
            officeId, retrieved.Count, reranked.Count, answer.IsRefusal);

        return new RagResult(answer, retrieved, reranked);
    }

    // Place the most relevant chunks at the start and end and the least relevant in the middle, where
    // models attend least ("lost in the middle"). Input is relevance-ordered (best first): even-ranked
    // chunks fill from the front, odd-ranked from the back, so rank-0 and rank-1 land on the two edges.
    // e.g. [r0,r1,r2,r3,r4] -> [r0,r2,r4,r3,r1].
    private static IReadOnlyList<ContextSource> ToEdgeOrderedContext(IReadOnlyList<RetrievedCandidate> reranked)
    {
        List<ContextSource> front = [];
        List<ContextSource> back = [];
        for (int i = 0; i < reranked.Count; i++)
        {
            (i % 2 == 0 ? front : back).Add(reranked[i].ToContextSource());
        }

        back.Reverse();
        front.AddRange(back);
        return front;
    }
}
