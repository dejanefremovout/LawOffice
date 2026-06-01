using LawOffice.AI.Prompts;

namespace LawOffice.AI.Tests.Prompts;

public class EmbeddedResourcePromptStoreTests
{
    private readonly EmbeddedResourcePromptStore _store = new();

    [Fact]
    public void Loads_terse_version_with_parsed_metadata_and_body()
    {
        PromptTemplate template = _store.GetPrompt("legal-assistant", "v1-terse");

        template.Name.ShouldBe("legal-assistant");
        template.Version.ShouldBe("v1-terse");
        template.Description.ShouldNotBeNullOrWhiteSpace();
        template.Body.ShouldContain("{{context}}");
        template.Body.ShouldContain("{{question}}");
    }

    [Fact]
    public void Loads_both_versions_of_the_same_prompt_name_and_they_differ()
    {
        PromptTemplate terse = _store.GetPrompt("legal-assistant", "v1-terse");
        PromptTemplate fewshot = _store.GetPrompt("legal-assistant", "v2-fewshot");

        fewshot.Name.ShouldBe(terse.Name);
        fewshot.Version.ShouldNotBe(terse.Version);
        fewshot.Body.ShouldContain("Example"); // the few-shot variant carries worked examples
        terse.Body.ShouldNotContain("Example");
    }

    [Fact]
    public void GetPrompt_throws_for_unknown_name_or_version()
    {
        PromptNotFoundException ex =
            Should.Throw<PromptNotFoundException>(() => _store.GetPrompt("legal-assistant", "v9-missing"));

        ex.Name.ShouldBe("legal-assistant");
        ex.Version.ShouldBe("v9-missing");
    }

    [Fact]
    public void TryGetPrompt_returns_false_for_unknown_prompt()
    {
        bool found = _store.TryGetPrompt("does-not-exist", "v1", out PromptTemplate? template);

        found.ShouldBeFalse();
        template.ShouldBeNull();
    }
}
