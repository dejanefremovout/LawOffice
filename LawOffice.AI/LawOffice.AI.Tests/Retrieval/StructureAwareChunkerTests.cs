using LawOffice.AI.Retrieval;

namespace LawOffice.AI.Tests.Retrieval;

public class StructureAwareChunkerTests
{
    private const string Doc =
        """
        # Residential Lease

        Intro paragraph before any clause.

        1. Term

        The term is twelve months.

        5. Termination

        The tenant shall give sixty days written notice before terminating this lease.
        """;

    [Fact]
    public void Splits_into_sections_along_clause_headings()
    {
        IReadOnlyList<DocumentChunk> chunks = new StructureAwareChunker().Chunk("lease-12", "lease", Doc);

        chunks.Select(c => c.Section).ShouldContain(s => s.Contains("Termination"));
        chunks.ShouldAllBe(c => c.DocId == "lease-12" && c.DocType == "lease");
    }

    [Fact]
    public void Keeps_a_clause_intact_when_it_fits_in_one_chunk()
    {
        IReadOnlyList<DocumentChunk> chunks = new StructureAwareChunker().Chunk("lease-12", "lease", Doc);

        DocumentChunk termination = chunks.First(c => c.Section.Contains("Termination"));
        termination.Text.ShouldContain("sixty days written notice");
    }

    [Fact]
    public void Splits_a_long_section_with_overlap_without_cutting_mid_sentence()
    {
        ChunkingOptions options = new() { MaxChunkChars = 120, OverlapChars = 30 };
        string body = "1. Long Clause\n" + string.Join(" ", Enumerable.Repeat("This sentence carries meaning.", 12));

        IReadOnlyList<DocumentChunk> chunks = new StructureAwareChunker(options).Chunk("d", "t", body);

        chunks.Count.ShouldBeGreaterThan(1);
        foreach (DocumentChunk chunk in chunks.SkipLast(1))
        {
            chunk.Text.TrimEnd().ShouldEndWith(".");
        }
    }

    [Fact]
    public void Blank_text_produces_no_chunks()
    {
        new StructureAwareChunker().Chunk("d", "t", "   ").ShouldBeEmpty();
    }
}
