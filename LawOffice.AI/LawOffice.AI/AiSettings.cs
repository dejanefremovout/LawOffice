namespace LawOffice.AI;

/// <summary>
/// Azure OpenAI connection settings, bound from the shared "AiSettings" configuration section.
/// These are the same keys the existing CaseManagement summarizer uses, so configuration is
/// shared across the repo (see CaseManagement.Api/local.settings.json).
/// </summary>
public sealed class AiSettings
{
    public const string SectionName = "AiSettings";

    /// <summary>Azure OpenAI resource endpoint, e.g. https://my-resource.openai.azure.com.</summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>Azure OpenAI API key.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Deployment (model) name to target, e.g. gpt-4.1-mini.</summary>
    public string DeploymentName { get; set; } = string.Empty;

    /// <summary>Azure OpenAI REST API version.</summary>
    public string ApiVersion { get; set; } = "2024-10-21";

    /// <summary>
    /// Embedding deployment (model) name for retrieval. text-embedding-3-small is cheap (~$0.00002/1K
    /// tokens) and emits 1,536-dim vectors — negligible cost for a demo-scale index.
    /// </summary>
    public string EmbeddingDeploymentName { get; set; } = "text-embedding-3-small";

    /// <summary>Dimensions of the embedding model's output; must match the vector index definition.</summary>
    public int EmbeddingDimensions { get; set; } = 1536;

    /// <summary>
    /// Throws when any required value is missing, mirroring the early-validation discipline of
    /// the existing AzureOpenAiSummarizerClient.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Endpoint) ||
            string.IsNullOrWhiteSpace(ApiKey) ||
            string.IsNullOrWhiteSpace(DeploymentName))
        {
            throw new InvalidOperationException(
                "AI settings are not configured. Set AiSettings:Endpoint, ApiKey and DeploymentName.");
        }
    }
}
