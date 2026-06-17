using System.Text;
using System.Text.RegularExpressions;

namespace LawOffice.AI.Prompts;

/// <summary>
/// Renders a <see cref="PromptTemplate"/> by substituting <c>{{slot}}</c> placeholders with values.
///
/// Two deliberate safety properties:
/// <list type="bullet">
/// <item>Any placeholder left unfilled after substitution throws — a missing slot is a bug, never a
/// silently-empty prompt.</item>
/// <item>Slot values are treated as untrusted <em>data</em>: occurrences of the data-fence delimiters
/// are neutralised so a malicious document (e.g. a poisoned legal file flowing in as context) cannot
/// "close" its fence and have the rest of its text read as instructions. This is a partial
/// prompt-injection mitigation (the boundary, not a cure) and foreshadows future security work.</item>
/// </list>
/// </summary>
public static partial class PromptRenderer
{
    /// <summary>Prefix of the line that opens an untrusted-data fence in a template.</summary>
    public const string FenceOpenPrefix = "----BEGIN PROVIDED";

    /// <summary>Prefix of the line that closes an untrusted-data fence in a template.</summary>
    public const string FenceClosePrefix = "----END PROVIDED";

    [GeneratedRegex(@"\{\{\s*(?<name>[A-Za-z0-9_]+)\s*\}\}")]
    private static partial Regex PlaceholderRegex();

    /// <summary>
    /// Substitutes every <c>{{slot}}</c> in the template body with its (sanitised) value.
    /// </summary>
    /// <exception cref="ArgumentException">A placeholder has no corresponding value in <paramref name="slots"/>.</exception>
    public static string Render(PromptTemplate template, IReadOnlyDictionary<string, string> slots)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(slots);

        return PlaceholderRegex().Replace(template.Body, match =>
        {
            string name = match.Groups["name"].Value;
            if (!slots.TryGetValue(name, out string? value))
            {
                throw new ArgumentException(
                    $"Prompt '{template.Name}/{template.Version}' has unfilled slot '{{{{{name}}}}}'.",
                    nameof(slots));
            }

            return Sanitize(value);
        });
    }

    /// <summary>
    /// Defangs fence-delimiter lines inside an untrusted value so it cannot break out of its data
    /// fence. The delimiter text is preserved (so nothing is silently lost) but broken up so it no
    /// longer matches a real fence line.
    /// </summary>
    private static string Sanitize(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        StringBuilder sb = new(value.Length);
        sb.Append(value);
        sb.Replace(FenceClosePrefix, DefangString(FenceClosePrefix));
        sb.Replace(FenceOpenPrefix, DefangString(FenceOpenPrefix));
        return sb.ToString();
    }

    // Insert a space after the leading dashes so "----END UNTRUSTED" becomes "---- END UNTRUSTED",
    // which is visible to a human reader but no longer matches a fence line the model treats as a boundary.
    private static string DefangString(string prefix) => prefix.Replace("----", "---- ");
}
