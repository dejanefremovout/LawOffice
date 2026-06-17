namespace LawOffice.AI.Retrieval;

/// <summary>
/// Size limits for chunking. Sizes are in characters (~4 chars/token in English; legal Serbian/English
/// mixed text runs differently — measure it), kept small so a single clause stays a single, precise unit.
/// </summary>
public sealed class ChunkingOptions
{
    /// <summary>Maximum characters per chunk before a section is split further. ~1200 chars ≈ ~300 tokens.</summary>
    public int MaxChunkChars { get; set; } = 1200;

    /// <summary>Characters of overlap carried between size-split chunks, to soften chunk-boundary damage.</summary>
    public int OverlapChars { get; set; } = 150;
}
