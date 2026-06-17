namespace LawOffice.AI.Prompts;

/// <summary>
/// An externalised, versioned prompt loaded from the prompt registry. Each template is
/// self-describing (name + version + description live in the source file's header), so prompts
/// can be iterated, A/B tested, rolled back and audited independently of application code.
/// </summary>
/// <param name="Name">Logical prompt name, stable across versions (e.g. "legal-assistant").</param>
/// <param name="Version">Version label for this variant (e.g. "v1-terse", "v2-fewshot").</param>
/// <param name="Description">Human-readable summary of what this version does / why it exists.</param>
/// <param name="Body">The template text, containing <c>{{slot}}</c> placeholders to be rendered.</param>
public sealed record PromptTemplate(string Name, string Version, string Description, string Body);
