using Azure;
using Azure.AI.OpenAI;
using LawOffice.AI.Assistant;
using LawOffice.AI.Observability;
using LawOffice.AI.Prompts;
using LawOffice.AI.Resilience;
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

        return services;
    }

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
