namespace LawOffice.AI.Prompts;

/// <summary>
/// Thrown when a prompt cannot be resolved by name + version. A missing prompt is a configuration
/// error (the registry shipped without an expected template), so it fails loudly rather than
/// silently falling back to a default.
/// </summary>
public sealed class PromptNotFoundException(string name, string version)
    : Exception($"No prompt found for name '{name}' version '{version}'.")
{
    public string Name { get; } = name;

    public string Version { get; } = version;
}
