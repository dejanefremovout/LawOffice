namespace LawOffice.AI.Retrieval;

/// <summary>
/// A retrieved record with its store-native similarity score. Score semantics are store-specific
/// (in-memory uses cosine similarity, higher = closer; Cosmos uses <c>VectorDistance</c>), so it's a
/// diagnostic for ranking/logging rather than a value to compare across stores.
/// </summary>
public sealed record ScoredChunk(VectorRecord Record, double Score);
