namespace LawOffice.AI.Retrieval;

/// <summary>
/// Splits a document's text into retrieval-sized <see cref="DocumentChunk"/>s. The strategy is the
/// quality lever: structure-aware splitting that respects clause/section boundaries massively
/// outperforms naive fixed-size splitting on legal text.
/// </summary>
public interface IDocumentChunker
{
    /// <summary>
    /// Chunks <paramref name="text"/> for the given document.
    /// </summary>
    /// <param name="docId">Source document identifier, copied onto every produced chunk.</param>
    /// <param name="docType">Document classification (e.g. "lease"), copied onto every produced chunk.</param>
    /// <param name="text">The full document text.</param>
    IReadOnlyList<DocumentChunk> Chunk(string docId, string docType, string text);
}
