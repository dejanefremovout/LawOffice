namespace LawOffice.AI.Prompts;

/// <summary>
/// A prompt registry: resolves externalised, versioned prompt templates by name + version.
/// This is the architectural alternative to scattering prompt string literals through the codebase
/// — prompts become content that can be versioned, rolled back, audited and (later) eval-gated
/// independently of the application binary.
/// </summary>
public interface IPromptStore
{
    /// <summary>
    /// Returns the template for the given name + version, or throws <see cref="PromptNotFoundException"/>
    /// if it is not registered.
    /// </summary>
    PromptTemplate GetPrompt(string name, string version);

    /// <summary>
    /// Attempts to resolve a template; returns false (and a null <paramref name="template"/>) when the
    /// name + version is not registered.
    /// </summary>
    bool TryGetPrompt(string name, string version, out PromptTemplate? template);
}
