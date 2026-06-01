using LawOffice.AI.Assistant;

namespace LawOffice.AI.Tests.Assistant;

public class LegalAnswerValidatorTests
{
    private readonly LegalAnswerValidator _validator = new();

    private static readonly IReadOnlyList<ContextSource> Context =
        [new("doc-1", "the source text"), new("doc-2", "more text")];

    [Fact]
    public void Grounded_answer_with_known_citation_is_valid()
    {
        LegalAnswer answer = new()
        {
            Answer = "Here is the grounded answer.",
            Citations = [new Citation("doc-1", "the source text")],
            Confidence = Confidence.High,
        };

        _validator.Validate(answer, Context).IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Clean_refusal_is_valid()
    {
        LegalAnswer answer = LegalAnswer.Refusal("The context does not cover this.");

        _validator.Validate(answer, Context).IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Answer_without_any_citation_is_invalid()
    {
        LegalAnswer answer = new() { Answer = "Unsupported claim.", Citations = [] };

        ValidationResult result = _validator.Validate(answer, Context);

        result.IsValid.ShouldBeFalse();
        result.Violations.ShouldContain(v => v.Contains("cite at least one"));
    }

    [Fact]
    public void Citation_to_a_source_not_in_context_is_invalid()
    {
        LegalAnswer answer = new()
        {
            Answer = "Confidently wrong.",
            Citations = [new Citation("ghost-99")],
            Confidence = Confidence.High,
        };

        ValidationResult result = _validator.Validate(answer, Context);

        result.IsValid.ShouldBeFalse();
        result.Violations.ShouldContain(v => v.Contains("ghost-99"));
    }

    [Fact]
    public void Refusal_that_also_carries_an_answer_is_invalid()
    {
        LegalAnswer answer = new()
        {
            Answer = "smuggled answer",
            Citations = [new Citation("doc-1")],
            RefusedReason = "but also refusing",
        };

        ValidationResult result = _validator.Validate(answer, Context);

        result.IsValid.ShouldBeFalse();
        result.Violations.Count.ShouldBe(2); // non-empty answer AND non-empty citations on a refusal
    }
}
