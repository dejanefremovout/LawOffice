using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using LawOffice.AI.Assistant;
using LawOffice.AI.Observability;
using LawOffice.AI.Prompts;
using LawOffice.AI.Resilience;
using LawOffice.AI.Retrieval;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LawOffice.AI.Extensions;

/// <summary>
/// Dependency injection registration for the LawOffice AI library.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a single, provider-abstracted <see cref="IChatClient"/> composed of:
    /// resilience (timeout/retry/circuit-breaker) → usage/cost logging → standard logging → Azure OpenAI.
    /// Registered as a singleton (the underlying SDK client is thread-safe).
    /// </summary>
    public static IServiceCollection AddLawOfficeAi(this IServiceCollection services, IConfiguration configuration)
    {
        AiSettings settings = configuration.GetSection(AiSettings.SectionName).Get<AiSettings>() ?? new AiSettings();
        settings.Validate();

        AiResilienceOptions resilience =
            configuration.GetSection(AiResilienceOptions.SectionName).Get<AiResilienceOptions>() ?? new AiResilienceOptions();
        AiCostSettings cost =
            configuration.GetSection(AiCostSettings.SectionName).Get<AiCostSettings>() ?? new AiCostSettings();

        services.AddSingleton<IChatClient>(sp =>
            BuildChatClient(settings, resilience, cost, sp.GetRequiredService<ILoggerFactory>()));

        AddLegalAssistant(services, configuration);
        services.AddRetrieval(configuration);

        return services;
    }

    /// <summary>
    /// Registers the retrieval layer: the embedding generator, a structure-aware chunker, the
    /// tenant-scoped <see cref="IVectorStore"/> (in-memory by default, Cosmos when configured), and the
    /// <see cref="TenantDocumentRetriever"/> that feeds retrieved context into the legal assistant.
    /// </summary>
    public static IServiceCollection AddRetrieval(this IServiceCollection services, IConfiguration configuration)
    {
        AiSettings settings = configuration.GetSection(AiSettings.SectionName).Get<AiSettings>() ?? new AiSettings();
        RetrievalSettings retrieval =
            configuration.GetSection(RetrievalSettings.SectionName).Get<RetrievalSettings>() ?? new RetrievalSettings();

        services.AddSingleton(retrieval);

        // Lazily built (like the chat client): constructing the Azure client makes no network call, so the
        // in-memory path and unit tests work with placeholder credentials.
        services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(_ => BuildEmbeddingGenerator(settings));
        services.AddSingleton<IDocumentChunker>(_ => new StructureAwareChunker());

        if (retrieval.UsesCosmos)
        {
            services.AddSingleton(_ => CreateCosmosClient(retrieval));
            services.AddSingleton<IVectorStore>(sp => new CosmosVectorStore(
                sp.GetRequiredService<CosmosClient>(),
                retrieval,
                settings.EmbeddingDimensions,
                sp.GetRequiredService<ILoggerFactory>().CreateLogger<CosmosVectorStore>()));
        }
        else
        {
            services.AddSingleton<IVectorStore, InMemoryVectorStore>();
        }

        services.AddSingleton<TenantDocumentRetriever>();
        return services;
    }

    /// <summary>
    /// Builds the Azure OpenAI embedding generator behind the provider-agnostic
    /// <see cref="IEmbeddingGenerator{TInput,TEmbedding}"/>, mirroring how <see cref="BuildChatClient"/>
    /// builds the chat client.
    /// </summary>
    public static IEmbeddingGenerator<string, Embedding<float>> BuildEmbeddingGenerator(AiSettings settings)
    {
        settings.Validate();
        AzureOpenAIClient azureClient = new(new Uri(settings.Endpoint), new AzureKeyCredential(settings.ApiKey));
        return azureClient.GetEmbeddingClient(settings.EmbeddingDeploymentName).AsIEmbeddingGenerator();
    }

    // Cosmos serializes our records with System.Text.Json (Web defaults) so the [JsonPropertyName]
    // lower-camel names — including the /officeId partition path — are honoured.
    private static CosmosClient CreateCosmosClient(RetrievalSettings retrieval)
    {
        CosmosClientOptions options = new()
        {
            UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web),
        };

        // The local Cosmos DB Emulator can't serve Direct/rntbd backend address resolution: reads come
        // back over the gateway, but the first write fails with a 400 on "/addresses/?...protocol eq rntbd".
        // Gateway mode routes everything through the single gateway port and avoids that. Real Azure keeps
        // the default Direct mode for throughput.
        if (IsCosmosEmulator(retrieval.CosmosConnectionString))
        {
            options.ConnectionMode = ConnectionMode.Gateway;
        }

        return new CosmosClient(retrieval.CosmosConnectionString, options);
    }

    // Well-known Cosmos DB Emulator master key, plus the loopback endpoints it listens on.
    private static bool IsCosmosEmulator(string connectionString) =>
        connectionString.Contains("C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", StringComparison.Ordinal)
        || connectionString.Contains("//localhost:", StringComparison.OrdinalIgnoreCase)
        || connectionString.Contains("//127.0.0.1:", StringComparison.Ordinal);

    /// <summary>
    /// Registers the prompt registry and the contract-validated legal assistant. Prompts are loaded
    /// once from embedded resources (immutable), so the store and the stateless validator/assistant are
    /// singletons; the only mutable dependency is the thread-safe singleton <see cref="IChatClient"/>.
    /// </summary>
    private static void AddLegalAssistant(IServiceCollection services, IConfiguration configuration)
    {
        LegalAssistantOptions options =
            configuration.GetSection(LegalAssistantOptions.SectionName).Get<LegalAssistantOptions>()
            ?? new LegalAssistantOptions();
        options.Validate();

        services.AddSingleton(options);
        services.AddSingleton<IPromptStore>(_ => new EmbeddedResourcePromptStore());
        services.AddSingleton<LegalAnswerValidator>();
        services.AddSingleton<ILegalAssistant, LegalAssistant>();
    }

    /// <summary>
    /// Builds the full <see cref="IChatClient"/> pipeline. Exposed so callers (e.g. the harness "chaos"
    /// demo) can construct a client with overridden resilience options without going through DI.
    /// </summary>
    public static IChatClient BuildChatClient(
        AiSettings settings,
        AiResilienceOptions resilience,
        AiCostSettings cost,
        ILoggerFactory loggerFactory)
    {
        settings.Validate();

        // ApiVersion is kept in settings for parity with the existing summarizer config; the Azure SDK
        // selects a default service version, so we don't map the string here.
        AzureOpenAIClient azureClient = new(new Uri(settings.Endpoint), new AzureKeyCredential(settings.ApiKey));
        IChatClient inner = azureClient.GetChatClient(settings.DeploymentName).AsIChatClient();

        // First .Use is the outermost wrapper: resilience sits outside usage logging, so a retried call
        // is metered once (on the successful attempt), while retries/breaker events log separately.
        return new ChatClientBuilder(inner)
            .Use((c, _) => new ResilientChatClient(c, resilience, loggerFactory.CreateLogger<ResilientChatClient>()))
            .Use((c, _) => new UsageLoggingChatClient(c, cost, loggerFactory.CreateLogger<UsageLoggingChatClient>()))
            .UseLogging(loggerFactory)
            .Build();
    }
}
