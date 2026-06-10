namespace LawOffice.AI.Retrieval;

/// <summary>
/// Reorders a broad candidate set into a tight, high-precision top-K. The single highest-ROI quality
/// upgrade after hybrid search: vector recall is cheap but imprecise, so we retrieve generously and let
/// a more discerning signal reorder before the candidates reach the model (and the context budget).
///
/// <para>
/// A pluggable seam: the default <see cref="LexicalReranker"/> is deterministic and free; a
/// cross-encoder or LLM reranker can drop in behind the same interface without touching the pipeline.
/// </para>
/// </summary>
public interface IReranker
{
    /// <summary>
    /// Returns the best <paramref name="topK"/> of <paramref name="candidates"/> for
    /// <paramref name="query"/>, most-relevant first. Never returns more than were supplied.
    /// </summary>
    IReadOnlyList<RetrievedCandidate> Rerank(
        string query,
        IReadOnlyList<RetrievedCandidate> candidates,
        int topK);
}
