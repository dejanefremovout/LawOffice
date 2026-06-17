using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace LawOffice.AI.Assistant;

/// <summary>
/// The strict output contract for the legal-document assistant — the typed shape the model is asked
/// to produce and that we validate ruthlessly at the service boundary. A "refusal" is a first-class,
/// valid outcome: empty <see cref="Answer"/> + empty <see cref="Citations"/> + a <see cref="RefusedReason"/>.
/// </summary>
public sealed record LegalAnswer
{
    /// <summary>The grounded answer, or empty when refusing.</summary>
    public string Answer { get; init; } = string.Empty;

    /// <summary>Sources supporting the answer; empty when refusing.</summary>
    public IReadOnlyList<Citation> Citations { get; init; } = [];

    /// <summary>The model's self-reported confidence.</summary>
    public Confidence Confidence { get; init; } = Confidence.Low;

    /// <summary>Why the assistant refused, or null when it answered.</summary>
    public string? RefusedReason { get; init; }

    /// <summary>True when this is a refusal rather than an answer.</summary>
    [JsonIgnore]
    public bool IsRefusal => !string.IsNullOrWhiteSpace(RefusedReason);

    /// <summary>Builds a safe refusal: no answer, no citations, low confidence.</summary>
    public static LegalAnswer Refusal(string reason) => new()
    {
        Answer = string.Empty,
        Citations = [],
        Confidence = Confidence.Low,
        RefusedReason = reason,
    };

    /// <summary>
    /// Shared serializer settings for the contract: camelCase property names, enums as strings, and
    /// case-insensitive reads (the model may vary casing). Used for both schema-shaped requests and
    /// for pretty-printing in the harness.
    /// </summary>
    public static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        // An explicit resolver is required because Microsoft.Extensions.AI marks these options read-only
        // when generating the response schema.
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true,
    };
}
