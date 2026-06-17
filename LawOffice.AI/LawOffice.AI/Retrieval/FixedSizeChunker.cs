namespace LawOffice.AI.Retrieval;

/// <summary>
/// Naive fixed-size chunker: ignores document structure and slices the whole text into ~equal windows
/// with overlap. Kept deliberately as the baseline for the Day-3 "structure vs fixed" comparison — read
/// the two side by side and you can see why structure-aware chunking wins on legal text. Not for production.
/// </summary>
public sealed class FixedSizeChunker : IDocumentChunker
{
    private const string SectionLabel = "fixed-window";

    private readonly int _windowChars;
    private readonly int _overlapChars;

    /// <param name="windowChars">Target window size in characters. ~2000 chars ≈ ~500 tokens (the bootcamp's baseline).</param>
    /// <param name="overlapChars">Overlap carried between windows.</param>
    public FixedSizeChunker(int windowChars = 2000, int overlapChars = 200)
    {
        _windowChars = windowChars;
        _overlapChars = overlapChars;
    }

    public IReadOnlyList<DocumentChunk> Chunk(string docId, string docType, string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(docId);
        ArgumentNullException.ThrowIfNull(text);

        List<DocumentChunk> chunks = [];
        int ordinal = 0;
        foreach (string window in TextWindowing.Split(text, _windowChars, _overlapChars))
        {
            chunks.Add(new DocumentChunk(docId, $"{SectionLabel}-{ordinal}", ordinal, window, docType));
            ordinal++;
        }

        return chunks;
    }
}
