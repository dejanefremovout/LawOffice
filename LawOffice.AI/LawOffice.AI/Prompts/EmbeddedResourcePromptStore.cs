using System.Reflection;

namespace LawOffice.AI.Prompts;

/// <summary>
/// A prompt registry backed by <c>*.prompt.md</c> files embedded in the assembly. Prompts ship inside
/// the binary, versioned with the build; the store scans the assembly manifest once at construction,
/// parses each file's front-matter header, and indexes templates by (name, version).
///
/// Each prompt file looks like:
/// <code>
/// ---
/// name: legal-assistant
/// version: v1-terse
/// description: ...
/// ---
/// &lt;template body with {{slots}}&gt;
/// </code>
/// Templates are keyed off the parsed header (not the resource path), so renaming a file never breaks
/// resolution.
/// </summary>
public sealed class EmbeddedResourcePromptStore : IPromptStore
{
    private const string ResourceSuffix = ".prompt.md";

    private readonly Dictionary<(string Name, string Version), PromptTemplate> _templates =
        new(KeyComparer);

    private static readonly IEqualityComparer<(string, string)> KeyComparer =
        new TupleOrdinalIgnoreCaseComparer();

    /// <summary>
    /// Loads all embedded <c>*.prompt.md</c> templates from the given assembly (defaulting to the
    /// assembly that defines this store, where the bundled templates live).
    /// </summary>
    public EmbeddedResourcePromptStore(Assembly? assembly = null)
    {
        assembly ??= typeof(EmbeddedResourcePromptStore).Assembly;

        foreach (string resourceName in assembly.GetManifestResourceNames())
        {
            if (!resourceName.EndsWith(ResourceSuffix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            using Stream stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Could not open embedded resource '{resourceName}'.");
            using StreamReader reader = new(stream);
            string raw = reader.ReadToEnd();

            PromptTemplate template = Parse(resourceName, raw);
            var key = (template.Name, template.Version);
            if (!_templates.TryAdd(key, template))
            {
                throw new InvalidOperationException(
                    $"Duplicate prompt '{template.Name}/{template.Version}' (resource '{resourceName}').");
            }
        }
    }

    /// <inheritdoc />
    public PromptTemplate GetPrompt(string name, string version) =>
        TryGetPrompt(name, version, out PromptTemplate? template)
            ? template!
            : throw new PromptNotFoundException(name, version);

    /// <inheritdoc />
    public bool TryGetPrompt(string name, string version, out PromptTemplate? template) =>
        _templates.TryGetValue((name, version), out template);

    private static PromptTemplate Parse(string resourceName, string raw)
    {
        // Normalise line endings so header parsing is independent of how the file was checked out.
        string text = raw.Replace("\r\n", "\n");

        if (!text.StartsWith("---\n", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Prompt resource '{resourceName}' is missing its '---' front-matter header.");
        }

        int end = text.IndexOf("\n---", 4, StringComparison.Ordinal);
        if (end < 0)
        {
            throw new InvalidOperationException(
                $"Prompt resource '{resourceName}' has an unterminated front-matter header.");
        }

        string header = text.Substring(4, end - 4);
        string body = text[(end + 4)..].TrimStart('\n');

        string? name = null, version = null, description = null;
        foreach (string line in header.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            int colon = line.IndexOf(':');
            if (colon <= 0)
            {
                continue;
            }

            string key = line[..colon].Trim();
            string value = line[(colon + 1)..].Trim();
            switch (key.ToLowerInvariant())
            {
                case "name": name = value; break;
                case "version": version = value; break;
                case "description": description = value; break;
            }
        }

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(version))
        {
            throw new InvalidOperationException(
                $"Prompt resource '{resourceName}' must declare both 'name' and 'version' in its header.");
        }

        return new PromptTemplate(name, version, description ?? string.Empty, body);
    }

    private sealed class TupleOrdinalIgnoreCaseComparer : IEqualityComparer<(string, string)>
    {
        public bool Equals((string, string) x, (string, string) y) =>
            StringComparer.OrdinalIgnoreCase.Equals(x.Item1, y.Item1) &&
            StringComparer.OrdinalIgnoreCase.Equals(x.Item2, y.Item2);

        public int GetHashCode((string, string) obj) =>
            HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Item1),
                StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Item2));
    }
}
