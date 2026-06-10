using LawOffice.AI.Assistant;

namespace LawOffice.AI.Retrieval;

/// <summary>
/// A retrieved chunk together with the signals a reranker needs to reorder it: the raw vector
/// similarity from the store and the chunk text/metadata. Distinct from <see cref="ContextSource"/>
/// (which is the citation-ready shape the assistant consumes) because reranking happens <i>before</i>
/// the candidate set is narrowed and handed off — the score and text are what the reranker fuses on.
/// </summary>
/// <param name="DocId">Source document id.</param>
/// <param name="Section">Clause/section label within the document.</param>
/// <param name="Text">The chunk text — the lexical signal for reranking and the answer grounding.</param>
/// <param name="VectorScore">Cosine similarity from the vector store (higher = more similar).</param>
public sealed record RetrievedCandidate(string DocId, string Section, string Text, double VectorScore)
{
    /// <summary>The citation id the assistant validates against, matching <see cref="TenantDocumentRetriever"/>.</summary>
    public string SourceId => $"{DocId}#{Section}";

    /// <summary>Projects this candidate into the citation-ready shape consumed by the assistant.</summary>
    public ContextSource ToContextSource() => new(SourceId, Text);
}
