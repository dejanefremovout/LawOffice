using LawOffice.AI.Observability;
using Microsoft.Extensions.AI;
using NSubstitute;

namespace LawOffice.AI.Tests.Observability;

public class UsageLoggingChatClientTests
{
    private static readonly List<ChatMessage> Messages =
        [new(ChatRole.User, "hello")];

    [Theory]
    [InlineData("gpt-4.1-mini", 1000, 500, 0.0012)] // 1000/1k*0.0004 + 500/1k*0.0016
    [InlineData("gpt-4.1-mini", 0, 0, 0.0)]
    public void Estimate_computes_cost_from_rate_table(string model, long input, long output, decimal expected)
    {
        AiCostSettings cost = new();

        cost.Estimate(model, input, output).ShouldBe(expected);
    }

    [Fact]
    public void Estimate_returns_null_for_unknown_model()
    {
        AiCostSettings cost = new();

        cost.Estimate("some-unlisted-model", 1000, 1000).ShouldBeNull();
    }

    [Fact]
    public async Task Logs_tokens_and_estimated_cost_and_passes_response_through()
    {
        ChatResponse expected = new(new ChatMessage(ChatRole.Assistant, "answer"))
        {
            ModelId = "gpt-4.1-mini",
            Usage = new UsageDetails { InputTokenCount = 1000, OutputTokenCount = 500 },
        };

        IChatClient inner = Substitute.For<IChatClient>();
        inner.GetResponseAsync(Arg.Any<IEnumerable<ChatMessage>>(), Arg.Any<ChatOptions?>(), Arg.Any<CancellationToken>())
            .Returns(expected);

        ListLogger<UsageLoggingChatClient> logger = new();
        UsageLoggingChatClient sut = new(inner, new AiCostSettings(), logger);

        ChatResponse result = await sut.GetResponseAsync(Messages);

        result.ShouldBe(expected); // pure observability middleware: response is forwarded unchanged

        string line = logger.Messages.ShouldHaveSingleItem();
        line.ShouldContain("model=gpt-4.1-mini");
        line.ShouldContain("inputTokens=1000");
        line.ShouldContain("outputTokens=500");
        line.ShouldContain("totalTokens=1500");
        line.ShouldContain("estimatedCostUsd=0.0012");
    }
}
