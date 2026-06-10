namespace LawOffice.AI.Retrieval;

/// <summary>
/// Splits an over-long passage into overlapping windows, preferring to break on a sentence/paragraph
/// boundary rather than mid-sentence. Shared by both chunkers so the size-splitting rule is identical.
/// </summary>
internal static class TextWindowing
{
    public static IReadOnlyList<string> Split(string text, int maxChars, int overlapChars)
    {
        string trimmed = text.Trim();
        if (trimmed.Length <= maxChars)
        {
            return trimmed.Length == 0 ? [] : [trimmed];
        }

        // Keep overlap strictly smaller than the window so the start always advances.
        overlapChars = Math.Clamp(overlapChars, 0, maxChars - 1);

        List<string> windows = [];
        int start = 0;
        while (start < trimmed.Length)
        {
            int hardEnd = Math.Min(start + maxChars, trimmed.Length);
            int end = hardEnd < trimmed.Length ? FindBreak(trimmed, start, hardEnd) : hardEnd;

            string window = trimmed[start..end].Trim();
            if (window.Length > 0)
            {
                windows.Add(window);
            }

            if (end >= trimmed.Length)
            {
                break;
            }

            // Step back by the overlap, but never past the start of the window we just emitted.
            start = Math.Max(end - overlapChars, start + 1);
        }

        return windows;
    }

    // Prefer the last sentence terminator (or newline) inside the window; fall back to the hard cut so a
    // run with no punctuation still makes progress.
    private static int FindBreak(string text, int start, int hardEnd)
    {
        int minBreak = start + (hardEnd - start) / 2; // don't produce a tiny sliver of a window
        for (int i = hardEnd - 1; i > minBreak; i--)
        {
            char c = text[i];
            bool sentenceEnd = (c is '.' or '!' or '?' or '\n') &&
                               (i + 1 >= text.Length || char.IsWhiteSpace(text[i + 1]));
            if (sentenceEnd)
            {
                return i + 1;
            }
        }

        return hardEnd;
    }
}
