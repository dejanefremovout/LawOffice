using System.ClientModel;
using LawOffice.AI.Assistant;
using LawOffice.AI.Prompts;
using Microsoft.Extensions.AI;

namespace LawOffice.AI.Tests.Assistant;

public class LegalAssistantTests
{
    private static readonly IReadOnlyList<ContextSource> Context =
        [new("lease-12", "The tenant shall give 60 days' written notice before terminating this lease.")];

    private const string ValidJson =
        """{"answer":"The tenant must give 60 days' written notice.","citations":[{"sourceId":"lease-12","quote":"60 days"}],"confidence":"High","refusedReason":null}""";

    private const string UngroundedJson =
        """{"answer":"Confidently wrong.","citations":[{"sourceId":"ghost-99"}],"confidence":"High","refusedReason":null}""";

    private const string RefusalJson =
        """{"answer":"","citations":[],"confidence":"Low","refusedReason":"Not in the supplied context."}""";

    private static ChatResponse Reply(string text) => new(new ChatMessage(ChatRole.Assistant, text));

    private static LegalAssistant Build(IChatClient inner, out ListLogger<LegalAssistant> logger)
    {
        logger = new ListLogger<LegalAssistant>();
        return new LegalAssistant(
            inner,
            new EmbeddedResourcePromptStore(),
            new LegalAnswerValidator(),
            new LegalAssistantOptions(), // legal-assistant / v1-terse / MaxRepairAttempts = 1
            logger);
    }

    [Fact]
    public async Task Returns_validated_answer_on_a_good_first_response()
    {
        FakeChatClient inner = new(_ => Reply(ValidJson));
        LegalAssistant sut = Build(inner, out ListLogger<LegalAssistant> logger);

        LegalAnswer answer = await sut.AnswerAsync("How much notice?", Context);

        answer.IsRefusal.ShouldBeFalse();
        answer.Answer.ShouldContain("60 days");
        answer.Confidence.ShouldBe(Confidence.High);
        answer.Citations.ShouldHaveSingleItem().SourceId.ShouldBe("lease-12");
        inner.CallCount.ShouldBe(1); // no repair needed
        logger.Messages.ShouldBeEmpty();
    }

    [Fact]
    public async Task Repairs_once_then_returns_the_corrected_answer()
    {
        FakeChatClient inner = new(call => call == 1 ? Reply(UngroundedJson) : Reply(ValidJson));
        LegalAssistant sut = Build(inner, out ListLogger<LegalAssistant> logger);

        LegalAnswer answer = await sut.AnswerAsync("How much notice?", Context);

        answer.IsRefusal.ShouldBeFalse();
        answer.Citations.ShouldHaveSingleItem().SourceId.ShouldBe("lease-12");
        inner.CallCount.ShouldBe(2); // initial + one repair
        logger.Messages.ShouldContain(m => m.Contains("repair attempt"));
    }

    [Fact]
    public async Task Falls_back_to_a_safe_refusal_when_repair_fails()
    {
        FakeChatClient inner = new(_ => Reply(UngroundedJson)); // never grounds its citation
        LegalAssistant sut = Build(inner, out ListLogger<LegalAssistant> logger);

        LegalAnswer answer = await sut.AnswerAsync("How much notice?", Context);

        answer.IsRefusal.ShouldBeTrue();
        answer.Answer.ShouldBeEmpty();
        answer.Citations.ShouldBeEmpty();
        answer.Confidence.ShouldBe(Confidence.Low);
        answer.RefusedReason.ShouldBe("Could not produce a contract-valid answer.");
        inner.CallCount.ShouldBe(2); // initial + MaxRepairAttempts(1)
        logger.Messages.ShouldContain(m => m.Contains("safe refusal"));
    }

    [Fact]
    public async Task Treats_unparseable_output_as_a_violation_and_repairs_without_throwing()
    {
        FakeChatClient inner = new(call => call == 1 ? Reply("this is not JSON") : Reply(ValidJson));
        LegalAssistant sut = Build(inner, out _);

        LegalAnswer answer = await sut.AnswerAsync("How much notice?", Context);

        answer.IsRefusal.ShouldBeFalse();
        inner.CallCount.ShouldBe(2);
    }

    [Fact]
    public async Task Passes_a_valid_refusal_straight_through()
    {
        FakeChatClient inner = new(_ => Reply(RefusalJson));
        LegalAssistant sut = Build(inner, out ListLogger<LegalAssistant> logger);

        LegalAnswer answer = await sut.AnswerAsync("Unrelated question?", Context);

        answer.IsRefusal.ShouldBeTrue();
        answer.RefusedReason.ShouldBe("Not in the supplied context.");
        inner.CallCount.ShouldBe(1);
        logger.Messages.ShouldBeEmpty(); // a legitimate refusal is not a repair or a fallback
    }

    [Fact]
    public async Task Resolves_the_requested_prompt_version()
    {
        FakeChatClient inner = new(_ => Reply(ValidJson));
        LegalAssistant sut = Build(inner, out _);

        LegalAnswer answer = await sut.AnswerAsync("How much notice?", Context, "v2-fewshot");

        answer.IsRefusal.ShouldBeFalse();
        inner.CallCount.ShouldBe(1);
    }

    [Fact]
    public async Task Refuses_safely_when_the_content_filter_blocks_the_request()
    {
        FakeChatClient inner = new(_ => throw new ClientResultException(
            "The response was filtered due to the prompt triggering Azure OpenAI's content management policy."));
        LegalAssistant sut = Build(inner, out ListLogger<LegalAssistant> logger);

        LegalAnswer answer = await sut.AnswerAsync("How much notice?", Context);

        answer.IsRefusal.ShouldBeTrue();
        answer.RefusedReason.ShouldBe("The request was blocked by the content safety filter.");
        answer.Confidence.ShouldBe(Confidence.Low);
        answer.Citations.ShouldBeEmpty();
        inner.CallCount.ShouldBe(1); // a content-filter block is deterministic, so it is not retried
        logger.Messages.ShouldContain(m => m.Contains("content safety filter"));
    }

    [Fact]
    public async Task Throws_when_the_requested_prompt_version_is_unknown()
    {
        FakeChatClient inner = new(_ => Reply(ValidJson));
        LegalAssistant sut = Build(inner, out _);

        await Should.ThrowAsync<PromptNotFoundException>(
            () => sut.AnswerAsync("How much notice?", Context, "v9-missing"));
    }
}
