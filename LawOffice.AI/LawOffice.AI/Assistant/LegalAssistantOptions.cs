namespace LawOffice.AI.Assistant;

/// <summary>
/// Options for the legal-document assistant, bound from the "AiSettings:LegalAssistant" section.
/// These tie the assistant to a prompt in the registry and govern the validate/repair behaviour.
/// </summary>
public sealed class LegalAssistantOptions
{
    public const string SectionName = "AiSettings:LegalAssistant";

    /// <summary>Logical prompt name to resolve from the <see cref="Prompts.IPromptStore"/>.</summary>
    public string PromptName { get; set; } = "legal-assistant";

    /// <summary>Prompt version used when a caller does not specify one.</summary>
    public string DefaultPromptVersion { get; set; } = "v1-terse";

    /// <summary>
    /// How many corrective re-calls to make when the model's output violates the contract before
    /// giving up and returning a safe refusal. Kept small because retries cost tokens.
    /// </summary>
    public int MaxRepairAttempts { get; set; } = 1;

    /// <summary>
    /// Whether to request a native JSON-schema response format. True works well with models that
    /// support structured outputs (e.g. gpt-4.1-mini); set false if a deployment/api-version rejects it.
    /// </summary>
    public bool UseJsonSchema { get; set; } = true;

    /// <summary>Throws when the options are not usable, mirroring the repo's early-validation discipline.</summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(PromptName) || string.IsNullOrWhiteSpace(DefaultPromptVersion))
        {
            throw new InvalidOperationException(
                "LegalAssistant options require PromptName and DefaultPromptVersion.");
        }

        if (MaxRepairAttempts < 0)
        {
            throw new InvalidOperationException("LegalAssistant MaxRepairAttempts cannot be negative.");
        }
    }
}
