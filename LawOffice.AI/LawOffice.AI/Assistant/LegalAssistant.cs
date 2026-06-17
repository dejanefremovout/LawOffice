using System.ClientModel;
using System.Text;
using LawOffice.AI.Prompts;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace LawOffice.AI.Assistant;

/// <summary>
/// Orchestrates a grounded, contract-validated legal answer:
/// resolve a versioned prompt → render it with delimited untrusted context/question →
/// ask the model for structured JSON output → validate against the contract →
/// repair once on violation → fall back to a safe refusal.
///
/// Treating the model's output as untrusted input that must satisfy a service-boundary contract is the
/// whole point: structured output gets the shape, the validator enforces the semantics, and a
/// safe "can't answer" always beats surfacing an unvalidated (possibly hallucinated) legal answer.
/// </summary>
public sealed class LegalAssistant : ILegalAssistant
{
    private const string SafeRefusalReason = "Could not produce a contract-valid answer.";
    private const string ContentFilterRefusalReason = "The request was blocked by the content safety filter.";

    private readonly IChatClient _chatClient;
    private readonly IPromptStore _promptStore;
    private readonly LegalAnswerValidator _validator;
    private readonly LegalAssistantOptions _options;
    private readonly ILogger<LegalAssistant> _logger;

    public LegalAssistant(
        IChatClient chatClient,
        IPromptStore promptStore,
        LegalAnswerValidator validator,
        LegalAssistantOptions options,
        ILogger<LegalAssistant> logger)
    {
        _chatClient = chatClient;
        _promptStore = promptStore;
        _validator = validator;
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<LegalAnswer> AnswerAsync(
        string question,
        IReadOnlyList<ContextSource> context,
        string? promptVersion = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(question);
        ArgumentNullException.ThrowIfNull(context);

        string version = promptVersion ?? _options.DefaultPromptVersion;
        PromptTemplate template = _promptStore.GetPrompt(_options.PromptName, version);

        string rendered = PromptRenderer.Render(template, new Dictionary<string, string>
        {
            ["context"] = FormatContext(context),
            ["question"] = question,
        });

        List<ChatMessage> messages = [new(ChatRole.User, rendered)];

        // Attempt 0 is the initial call; each subsequent attempt is a repair turn.
        for (int attempt = 0; ; attempt++)
        {
            ChatResponse<LegalAnswer> response;
            try
            {
                response = await _chatClient
                    .GetResponseAsync<LegalAnswer>(
                        messages,
                        LegalAnswer.SerializerOptions,
                        useJsonSchemaResponseFormat: _options.UseJsonSchema,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (ClientResultException ex) when (IsContentFilter(ex))
            {
                // The content safety filter is a guardrail firing, not a transient fault: legal documents
                // legitimately contain material it blocks, and re-sending the same prompt would just
                // trigger it again. Degrade to a safe refusal rather than crashing the request.
                _logger.LogWarning(ex, "Model call blocked by the content safety filter; returning a safe refusal.");
                return LegalAnswer.Refusal(ContentFilterRefusalReason);
            }

            IReadOnlyList<string> violations = Evaluate(response, context, out LegalAnswer? answer);
            if (violations.Count == 0)
            {
                if (attempt > 0)
                {
                    _logger.LogInformation(
                        "Legal answer satisfied the contract after {Attempts} repair attempt(s).", attempt);
                }

                return answer!;
            }

            if (attempt >= _options.MaxRepairAttempts)
            {
                _logger.LogWarning(
                    "Legal answer still violated the contract after {Attempts} repair attempt(s); " +
                    "returning a safe refusal. Violations: {Violations}",
                    attempt,
                    string.Join("; ", violations));

                return LegalAnswer.Refusal(SafeRefusalReason);
            }

            AppendRepairTurn(messages, response.Text, violations);
        }
    }

    // Parses + validates the response. Returns the list of violations (empty when valid) and, when
    // parseable, the parsed answer.
    private IReadOnlyList<string> Evaluate(
        ChatResponse<LegalAnswer> response,
        IReadOnlyList<ContextSource> context,
        out LegalAnswer? answer)
    {
        if (!response.TryGetResult(out answer) || answer is null)
        {
            // Malformed/non-JSON output is an expected contract violation, not an exception.
            return ["Model output was not valid JSON matching the required schema."];
        }

        ValidationResult result = _validator.Validate(answer, context);
        return result.IsValid ? [] : result.Violations;
    }

    // A content-filter block surfaces as a 400 ClientResultException whose payload names the policy
    // violation. Match on the documented markers (rather than only the status code) so the check is
    // resilient to SDK changes and to output-side filtering.
    private static bool IsContentFilter(ClientResultException ex)
    {
        string message = ex.Message;
        return message.Contains("content_filter", StringComparison.OrdinalIgnoreCase)
            || message.Contains("content management policy", StringComparison.OrdinalIgnoreCase)
            || message.Contains("ResponsibleAIPolicyViolation", StringComparison.OrdinalIgnoreCase);
    }

    private static void AppendRepairTurn(
        List<ChatMessage> messages,
        string previousOutput,
        IReadOnlyList<string> violations)
    {
        messages.Add(new ChatMessage(ChatRole.Assistant, previousOutput));
        messages.Add(new ChatMessage(
            ChatRole.User,
            "Your previous output violated the required contract: " +
            string.Join("; ", violations) +
            ". Return ONLY a single JSON object that matches the schema. If the context does not support " +
            "an answer, refuse by leaving \"answer\" empty, \"citations\" empty, and setting \"refusedReason\"."));
    }

    // Renders supplied sources as id-tagged blocks so the model can cite by id; the renderer sanitises
    // the (untrusted) source text as it substitutes the {{context}} slot.
    private static string FormatContext(IReadOnlyList<ContextSource> context)
    {
        if (context.Count == 0)
        {
            return "(no context supplied)";
        }

        StringBuilder sb = new();
        foreach (ContextSource source in context)
        {
            sb.Append("[source: ").Append(source.Id).Append("]\n");
            sb.Append(source.Text).Append("\n\n");
        }

        return sb.ToString().TrimEnd('\n');
    }
}
