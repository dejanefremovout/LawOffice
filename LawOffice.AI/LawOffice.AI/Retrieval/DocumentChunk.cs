namespace LawOffice.AI.Retrieval;

/// <summary>
/// A retrieval-sized piece of a source document, produced by an <see cref="IDocumentChunker"/>.
/// Chunking is where RAG quality is won or lost: too big dilutes retrieval, too small breaks clauses.
/// </summary>
/// <param name="DocId">Identifier of the source document this chunk came from (e.g. "lease-12").</param>
/// <param name="Section">
/// Human-meaningful section/clause label (e.g. "3. Termination"). Carried into the citation id so the
/// model cites a recognisable location rather than an opaque offset.
/// </param>
/// <param name="Ordinal">Zero-based position of this chunk within its section (for sections split by size).</param>
/// <param name="Text">The chunk text the model may answer from.</param>
/// <param name="DocType">Document classification metadata (e.g. "lease", "nda"), stored for filtering.</param>
public sealed record DocumentChunk(string DocId, string Section, int Ordinal, string Text, string DocType);
