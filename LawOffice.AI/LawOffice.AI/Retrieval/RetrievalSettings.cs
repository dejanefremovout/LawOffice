namespace LawOffice.AI.Retrieval;

/// <summary>
/// Configuration for the retrieval layer, bound from "AiSettings:Retrieval". Defaults to the zero-cost
/// in-memory provider so nothing needs provisioning to run the demo or the tests; set
/// <see cref="Provider"/> to "Cosmos" to use Azure Cosmos DB integrated vector search.
/// </summary>
public sealed class RetrievalSettings
{
    public const string SectionName = "AiSettings:Retrieval";

    /// <summary>"InMemory" (default) or "Cosmos".</summary>
    public string Provider { get; set; } = "InMemory";

    /// <summary>Cosmos connection string (only used when <see cref="Provider"/> is "Cosmos").</summary>
    public string CosmosConnectionString { get; set; } = string.Empty;

    /// <summary>Cosmos database id. Reuses the existing CaseManagement database by default.</summary>
    public string DatabaseId { get; set; } = "casemanagement";

    /// <summary>Cosmos container id holding the document chunks + embeddings.</summary>
    public string ContainerId { get; set; } = "documentchunks";

    /// <summary>Default number of chunks to retrieve per query.</summary>
    public int TopK { get; set; } = 5;

    public bool UsesCosmos => string.Equals(Provider, "Cosmos", StringComparison.OrdinalIgnoreCase);
}
