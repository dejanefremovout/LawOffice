using LawOffice.AI.Assistant;
using LawOffice.AI.Extensions;
using LawOffice.AI.Prompts;
using LawOffice.AI.Retrieval;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LawOffice.AI.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    private static ServiceProvider BuildProvider()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                // Placeholder Azure settings: the IChatClient is built lazily and makes no network call here.
                ["AiSettings:Endpoint"] = "https://unit-test.openai.azure.com",
                ["AiSettings:ApiKey"] = "unit-test-key",
                ["AiSettings:DeploymentName"] = "gpt-4.1-mini",
            })
            .Build();

        return new ServiceCollection()
            .AddLogging()
            .AddLawOfficeAi(configuration)
            .BuildServiceProvider();
    }

    [Fact]
    public void Registers_the_prompt_store_with_the_bundled_templates()
    {
        using ServiceProvider provider = BuildProvider();

        IPromptStore store = provider.GetRequiredService<IPromptStore>();

        store.GetPrompt("legal-assistant", "v1-terse").ShouldNotBeNull();
        store.GetPrompt("legal-assistant", "v2-fewshot").ShouldNotBeNull();
    }

    [Fact]
    public void Resolves_the_legal_assistant_and_its_dependencies()
    {
        using ServiceProvider provider = BuildProvider();

        provider.GetRequiredService<ILegalAssistant>().ShouldNotBeNull();
        provider.GetRequiredService<LegalAnswerValidator>().ShouldNotBeNull();
        provider.GetRequiredService<LegalAssistantOptions>().PromptName.ShouldBe("legal-assistant");
    }

    [Fact]
    public void Registers_the_in_memory_retrieval_stack_by_default()
    {
        using ServiceProvider provider = BuildProvider();

        provider.GetRequiredService<IVectorStore>().ShouldBeOfType<InMemoryVectorStore>();
        provider.GetRequiredService<IDocumentChunker>().ShouldBeOfType<StructureAwareChunker>();
        provider.GetRequiredService<TenantDocumentRetriever>().ShouldNotBeNull();
    }
}
