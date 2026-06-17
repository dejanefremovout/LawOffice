using LawOffice.AI.Assistant;

namespace LawOffice.AI.Retrieval;

/// <summary>
/// The outcome of a RAG query: the grounded <see cref="LegalAnswer"/> plus the retrieval diagnostics
/// that produced it. The diagnostics make the pipeline observable — which chunks were retrieved, their
/// scores, and which survived reranking — which is what you need to debug a confident wrong answer
/// (was it a retrieval miss or a generation/grounding fault?) and the seed of the eval/observability work.
/// </summary>
/// <param name="Answer">The contract-validated answer (or a safe refusal).</param>
/// <param name="Retrieved">The broad candidate pool from the vector store, most-similar first.</param>
/// <param name="Reranked">The candidates kept after reranking — the context actually shown to the model.</param>
public sealed record RagResult(
    LegalAnswer Answer,
    IReadOnlyList<RetrievedCandidate> Retrieved,
    IReadOnlyList<RetrievedCandidate> Reranked);
