namespace LawOffice.AI.Retrieval;

/// <summary>
/// Tenant-scoped vector storage and nearest-neighbour search.
///
/// <para>
/// Tenant isolation is enforced by the contract itself: <c>officeId</c> is a required first parameter
/// on every operation, so a caller cannot search or write without naming the tenant. Implementations
/// add a second, data-layer enforcement (Cosmos scopes by partition key <i>and</i> a
/// <c>WHERE c.officeId = @officeId</c> filter) — defence in depth, because in a regulated legal-tech
/// product a single missing filter would leak one client's privileged documents into another's results
/// (OWASP LLM "Vector and Embedding Weaknesses").
/// </para>
/// </summary>
public interface IVectorStore
{
    /// <summary>Inserts or replaces records for a single tenant. Every record must belong to <paramref name="officeId"/>.</summary>
    Task UpsertAsync(string officeId, IReadOnlyList<VectorRecord> records, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the <paramref name="topK"/> nearest records <b>within <paramref name="officeId"/> only</b>,
    /// ordered most-similar first. Records belonging to any other tenant are never considered.
    /// </summary>
    Task<IReadOnlyList<ScoredChunk>> SearchAsync(
        string officeId,
        ReadOnlyMemory<float> queryEmbedding,
        int topK,
        CancellationToken cancellationToken = default);
}
