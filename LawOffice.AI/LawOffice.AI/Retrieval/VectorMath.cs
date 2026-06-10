namespace LawOffice.AI.Retrieval;

/// <summary>Vector similarity helpers for the in-memory store.</summary>
internal static class VectorMath
{
    /// <summary>
    /// Cosine similarity in [-1, 1] (higher = more similar) — the metric the bootcamp names as the most
    /// common for semantic search. Returns 0 for a zero-length vector rather than dividing by zero.
    /// </summary>
    public static double CosineSimilarity(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
    {
        if (a.Length != b.Length)
        {
            throw new ArgumentException($"Vector length mismatch: {a.Length} vs {b.Length}.");
        }

        double dot = 0, normA = 0, normB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * (double)b[i];
            normA += a[i] * (double)a[i];
            normB += b[i] * (double)b[i];
        }

        if (normA == 0 || normB == 0)
        {
            return 0;
        }

        return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}
