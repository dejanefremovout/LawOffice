using System.Collections.Concurrent;

namespace LawOffice.AI.Retrieval;

/// <summary>
/// In-memory <see cref="IVectorStore"/> using brute-force cosine similarity. It is the default,
/// zero-cost, zero-infrastructure provider (no Azure required) and the substrate for the tenant-isolation
/// test: records are bucketed by tenant and a search only ever scans its own bucket, so a query for one
/// tenant cannot surface another tenant's vectors even when they are geometrically closer.
/// </summary>
public sealed class InMemoryVectorStore : IVectorStore
{
    // tenant -> (recordId -> record). Per-tenant buckets make the isolation boundary structural.
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, VectorRecord>> _byTenant = new();

    public Task UpsertAsync(string officeId, IReadOnlyList<VectorRecord> records, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentNullException.ThrowIfNull(records);

        ConcurrentDictionary<string, VectorRecord> bucket = _byTenant.GetOrAdd(officeId, _ => new());
        foreach (VectorRecord record in records)
        {
            if (record.OfficeId != officeId)
            {
                throw new ArgumentException(
                    $"Record '{record.Id}' has officeId '{record.OfficeId}' but is being upserted under '{officeId}'.",
                    nameof(records));
            }

            bucket[record.Id] = record;
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ScoredChunk>> SearchAsync(
        string officeId,
        ReadOnlyMemory<float> queryEmbedding,
        int topK,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(topK);

        if (!_byTenant.TryGetValue(officeId, out ConcurrentDictionary<string, VectorRecord>? bucket))
        {
            return Task.FromResult<IReadOnlyList<ScoredChunk>>([]);
        }

        ReadOnlyMemory<float> query = queryEmbedding;
        IReadOnlyList<ScoredChunk> results = bucket.Values
            .Select(r => new ScoredChunk(r, VectorMath.CosineSimilarity(r.Embedding, query.Span)))
            .OrderByDescending(s => s.Score)
            .Take(topK)
            .ToList();

        return Task.FromResult(results);
    }
}
