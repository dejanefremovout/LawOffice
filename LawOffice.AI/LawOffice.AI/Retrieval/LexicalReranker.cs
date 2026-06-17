using System.Text;

namespace LawOffice.AI.Retrieval;

/// <summary>
/// A deterministic, zero-cost reranker that fuses the vector similarity score with a lexical (BM25)
/// keyword-overlap signal computed over the candidate set. This <i>is</i> naive hybrid search: pure
/// vector recall misses exact identifiers (case numbers, statute references, party names) because they
/// carry little semantic signal, so a candidate that lexically matches the query is pulled up even when
/// cosine alone buried it. Both signals are min-max normalised across the candidate set before fusion
/// so neither scale dominates.
///
/// <para>
/// Deterministic and free, so it runs in CI without a model call. A cross-encoder or LLM reranker can
/// replace it behind <see cref="IReranker"/> when quality justifies the cost/latency.
/// </para>
/// </summary>
public sealed class LexicalReranker : IReranker
{
    // BM25 term-frequency saturation (k1) and length-normalisation (b) — standard defaults.
    private const double K1 = 1.5;
    private const double B = 0.75;

    private readonly double _vectorWeight;

    /// <param name="vectorWeight">
    /// Fusion weight on the (normalised) vector score in [0,1]; the lexical score gets the remainder.
    /// 0.5 (default) weights semantic and lexical evidence equally.
    /// </param>
    public LexicalReranker(double vectorWeight = 0.5)
    {
        if (vectorWeight is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(vectorWeight), vectorWeight, "Must be in [0, 1].");
        }

        _vectorWeight = vectorWeight;
    }

    public IReadOnlyList<RetrievedCandidate> Rerank(
        string query,
        IReadOnlyList<RetrievedCandidate> candidates,
        int topK)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(candidates);

        if (candidates.Count == 0 || topK <= 0)
        {
            return [];
        }

        double[] lexical = ScoreLexical(query, candidates);
        double[] vector = candidates.Select(c => c.VectorScore).ToArray();

        Normalise(lexical);
        Normalise(vector);

        // Stable, deterministic order: fused score desc, then raw vector desc, then source id.
        return candidates
            .Select((candidate, i) => (candidate, fused: (_vectorWeight * vector[i]) + ((1 - _vectorWeight) * lexical[i])))
            .OrderByDescending(x => x.fused)
            .ThenByDescending(x => x.candidate.VectorScore)
            .ThenBy(x => x.candidate.SourceId, StringComparer.Ordinal)
            .Take(topK)
            .Select(x => x.candidate)
            .ToList();
    }

    // BM25 of each candidate against the query, treating the candidate set as the corpus.
    private static double[] ScoreLexical(string query, IReadOnlyList<RetrievedCandidate> candidates)
    {
        List<string> queryTerms = Tokenize(query).Distinct(StringComparer.Ordinal).ToList();
        List<string>[] docs = candidates.Select(c => Tokenize(c.Text)).ToArray();

        double avgLength = docs.Average(d => d.Count);
        if (avgLength <= 0)
        {
            return new double[candidates.Count];
        }

        // Document frequency per query term, for idf.
        Dictionary<string, int> docFreq = new(StringComparer.Ordinal);
        foreach (string term in queryTerms)
        {
            docFreq[term] = docs.Count(d => d.Contains(term));
        }

        double[] scores = new double[candidates.Count];
        for (int i = 0; i < docs.Length; i++)
        {
            List<string> doc = docs[i];
            double lengthNorm = K1 * (1 - B + (B * doc.Count / avgLength));

            foreach (string term in queryTerms)
            {
                int tf = doc.Count(t => string.Equals(t, term, StringComparison.Ordinal));
                if (tf == 0)
                {
                    continue;
                }

                double idf = Math.Log(1 + ((candidates.Count - docFreq[term] + 0.5) / (docFreq[term] + 0.5)));
                scores[i] += idf * (tf * (K1 + 1) / (tf + lengthNorm));
            }
        }

        return scores;
    }

    // Min-max scale to [0,1]. A flat set (all equal, including all-zero) maps to all-zeros so the signal
    // simply doesn't discriminate, letting the other signal decide.
    private static void Normalise(double[] values)
    {
        double min = values.Min();
        double max = values.Max();
        double range = max - min;
        if (range <= double.Epsilon)
        {
            Array.Clear(values);
            return;
        }

        for (int i = 0; i < values.Length; i++)
        {
            values[i] = (values[i] - min) / range;
        }
    }

    // Lowercase word tokenizer: letters/digits form tokens, everything else splits. Keeps identifiers
    // like "12-cv-345" as the tokens "12", "cv", "345" so they overlap with a query naming them.
    private static List<string> Tokenize(string text)
    {
        List<string> tokens = [];
        StringBuilder current = new();
        foreach (char c in text)
        {
            if (char.IsLetterOrDigit(c))
            {
                current.Append(char.ToLowerInvariant(c));
            }
            else if (current.Length > 0)
            {
                tokens.Add(current.ToString());
                current.Clear();
            }
        }

        if (current.Length > 0)
        {
            tokens.Add(current.ToString());
        }

        return tokens;
    }
}
