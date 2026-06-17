using System.Text;
using System.Text.RegularExpressions;

namespace LawOffice.AI.Retrieval;

/// <summary>
/// Splits documents along their structure — markdown headings, numbered clauses, and
/// "Article/Section/Clause" headers (incl. common Serbian forms) — so each chunk is a coherent
/// clause/section rather than an arbitrary window. Long sections are size-split with overlap. For
/// legal text this respects clause boundaries and massively outperforms naive fixed-size chunking.
/// </summary>
public sealed partial class StructureAwareChunker : IDocumentChunker
{
    private const string PreambleSection = "Preamble";

    private readonly ChunkingOptions _options;

    public StructureAwareChunker(ChunkingOptions? options = null) => _options = options ?? new ChunkingOptions();

    public IReadOnlyList<DocumentChunk> Chunk(string docId, string docType, string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(docId);
        ArgumentNullException.ThrowIfNull(text);

        List<DocumentChunk> chunks = [];
        foreach ((string section, string body) in SplitIntoSections(text))
        {
            int ordinal = 0;
            foreach (string window in TextWindowing.Split(body, _options.MaxChunkChars, _options.OverlapChars))
            {
                chunks.Add(new DocumentChunk(docId, section, ordinal++, window, docType));
            }
        }

        return chunks;
    }

    // Groups consecutive lines under their nearest preceding heading. Content before the first heading
    // becomes a "Preamble" section so nothing is dropped.
    private static IEnumerable<(string Section, string Body)> SplitIntoSections(string text)
    {
        string normalized = text.Replace("\r\n", "\n").Replace('\r', '\n');
        string currentSection = PreambleSection;
        StringBuilder body = new();

        foreach (string line in normalized.Split('\n'))
        {
            Match heading = HeadingPattern().Match(line);
            if (heading.Success)
            {
                if (body.Length > 0)
                {
                    yield return (currentSection, body.ToString());
                    body.Clear();
                }

                currentSection = CleanHeading(heading.Value);
            }
            else
            {
                body.Append(line).Append('\n');
            }
        }

        if (body.Length > 0)
        {
            yield return (currentSection, body.ToString());
        }
    }

    // Strips leading markdown hashes/whitespace and caps the label length so it reads cleanly in a citation.
    private static string CleanHeading(string heading)
    {
        string cleaned = heading.Trim().TrimStart('#').Trim();
        return cleaned.Length <= 80 ? cleaned : cleaned[..80].TrimEnd();
    }

    // A heading is: a markdown "# .." line, a numbered "1." / "1.2 .." clause header, or a line starting
    // with Article/Section/Clause (and the Serbian "Член"/"Глава"/"Поглавље").
    [GeneratedRegex(
        @"^\s*(#{1,6}\s+\S.*|\d+(\.\d+)*\.?\s+\S.*|(Article|ARTICLE|Section|SECTION|Clause|CLAUSE|Члан|ЧЛАН|Глава|ГЛАВА|Поглавље|ПОГЛАВЉЕ)\b.*)$",
        RegexOptions.CultureInvariant)]
    private static partial Regex HeadingPattern();
}
