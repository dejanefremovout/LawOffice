using System.Text;
using System.Text.Json.Serialization;

namespace LawOffice.AI.Retrieval;

/// <summary>
/// A persisted chunk plus its embedding — the unit stored in and returned from an <see cref="IVectorStore"/>.
/// JSON property names are lower-camel to match the repo's Cosmos convention (the partition key is
/// <c>/officeId</c> across all containers), so the same document shape works against Cosmos directly.
/// </summary>
public sealed record VectorRecord
{
    /// <summary>Cosmos item id, unique within the tenant partition. Free of Cosmos-reserved characters.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>Tenant scope. The partition key and the mandatory filter on every query.</summary>
    [JsonPropertyName("officeId")]
    public required string OfficeId { get; init; }

    [JsonPropertyName("docId")]
    public required string DocId { get; init; }

    [JsonPropertyName("section")]
    public required string Section { get; init; }

    [JsonPropertyName("docType")]
    public required string DocType { get; init; }

    [JsonPropertyName("text")]
    public required string Text { get; init; }

    /// <summary>The embedding vector. Indexed for ANN search; excluded from normal Cosmos indexing.</summary>
    [JsonPropertyName("embedding")]
    public required float[] Embedding { get; init; }

    /// <summary>
    /// Builds a <see cref="VectorRecord"/> from a chunk, embedding, and tenant. The Cosmos id is derived
    /// from doc/section/ordinal and sanitised of the characters Cosmos forbids in an item id
    /// (<c>/ \ ? #</c>), keeping it deterministic so re-ingesting the same document upserts in place.
    /// </summary>
    public static VectorRecord FromChunk(string officeId, DocumentChunk chunk, ReadOnlyMemory<float> embedding) =>
        new()
        {
            Id = BuildId(chunk),
            OfficeId = officeId,
            DocId = chunk.DocId,
            Section = chunk.Section,
            DocType = chunk.DocType,
            Text = chunk.Text,
            Embedding = embedding.ToArray(),
        };

    private static string BuildId(DocumentChunk chunk)
    {
        StringBuilder sb = new(chunk.DocId.Length + chunk.Section.Length + 8);
        Append(sb, chunk.DocId);
        sb.Append("__");
        Append(sb, chunk.Section);
        sb.Append("__").Append(chunk.Ordinal);
        return sb.ToString();
    }

    // Replace Cosmos-reserved id characters (and whitespace) so the id is always a valid item id.
    private static void Append(StringBuilder sb, string value)
    {
        foreach (char c in value)
        {
            sb.Append(c is '/' or '\\' or '?' or '#' || char.IsWhiteSpace(c) ? '-' : c);
        }
    }
}
