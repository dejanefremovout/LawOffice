using LawOffice.AI.Assistant;
using Microsoft.Extensions.AI;

namespace LawOffice.AI.Retrieval;

/// <summary>
/// The seam between retrieval and the grounded assistant. Embeds a query (or a batch of chunks) with the
/// configured <see cref="IEmbeddingGenerator{TInput,TEmbedding}"/>, searches the tenant-scoped
/// <see cref="IVectorStore"/>, and maps hits to the <see cref="ContextSource"/> shape that
/// <see cref="ILegalAssistant"/> already consumes — so the Day-2 assistant gets its grounding context
/// from tenant-isolated retrieval without any change to the assistant itself.
/// </summary>
public sealed class TenantDocumentRetriever
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddings;
    private readonly IVectorStore _store;

    public TenantDocumentRetriever(IEmbeddingGenerator<string, Embedding<float>> embeddings, IVectorStore store)
    {
        _embeddings = embeddings;
        _store = store;
    }

    /// <summary>
    /// Embeds <paramref name="chunks"/> in one batch and upserts them under <paramref name="officeId"/>,
    /// returning the stored records. The tenant is fixed by the caller's authenticated identity — never by
    /// anything the model supplies.
    /// </summary>
    public async Task<IReadOnlyList<VectorRecord>> IngestAsync(
        string officeId,
        IReadOnlyList<DocumentChunk> chunks,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentNullException.ThrowIfNull(chunks);
        if (chunks.Count == 0)
        {
            return [];
        }

        GeneratedEmbeddings<Embedding<float>> vectors = await _embeddings
            .GenerateAsync(chunks.Select(c => c.Text), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        List<VectorRecord> records = new(chunks.Count);
        for (int i = 0; i < chunks.Count; i++)
        {
            records.Add(VectorRecord.FromChunk(officeId, chunks[i], vectors[i].Vector));
        }

        await _store.UpsertAsync(officeId, records, cancellationToken).ConfigureAwait(false);
        return records;
    }

    /// <summary>
    /// Embeds the query and returns the top-<paramref name="topK"/> chunks for <paramref name="officeId"/>
    /// as citation-ready <see cref="ContextSource"/>s (id = <c>{docId}#{section}</c>, the form the
    /// assistant's grounding check validates against).
    /// </summary>
    public async Task<IReadOnlyList<ContextSource>> RetrieveAsync(
        string officeId,
        string query,
        int topK,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        Embedding<float> queryEmbedding = await _embeddings
            .GenerateAsync(query, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        IReadOnlyList<ScoredChunk> hits = await _store
            .SearchAsync(officeId, queryEmbedding.Vector, topK, cancellationToken)
            .ConfigureAwait(false);

        return hits
            .Select(h => new ContextSource($"{h.Record.DocId}#{h.Record.Section}", h.Record.Text))
            .ToList();
    }
}
