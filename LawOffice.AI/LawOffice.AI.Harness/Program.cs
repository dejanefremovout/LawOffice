using LawOffice.AI;
using LawOffice.AI.Extensions;
using LawOffice.AI.Resilience;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Proves the "LLM = flaky external dependency" framing in code.
//   dotnet run -- stream   streaming completion to the console
//   dotnet run -- chaos    1s timeout -> retry (logged) -> graceful fallback
//   dotnet run -- cost     non-streaming call; token + cost line is logged
//
// Endpoint/ApiKey come from user-secrets or environment (AiSettings__Endpoint / AiSettings__ApiKey),
// never from source control.

const string SystemPrompt =
    "You are an assistant for a small law office. Be concise and factual. Do not provide legal advice.";

string mode = args.Length > 0 ? args[0].Trim().ToLowerInvariant() : "stream";

HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddLawOfficeAi(builder.Configuration);

using IHost host = builder.Build();
ILoggerFactory loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
ILogger logger = loggerFactory.CreateLogger("Harness");

switch (mode)
{
    case "chaos":
        await RunChaosAsync(builder.Configuration, loggerFactory, logger);
        break;
    case "cost":
        await RunCostAsync(host.Services.GetRequiredService<IChatClient>(), logger);
        break;
    default:
        await RunStreamAsync(host.Services.GetRequiredService<IChatClient>(), logger);
        break;
}

return;

async Task RunStreamAsync(IChatClient client, ILogger log)
{
    log.LogInformation("=== STREAM demo: latency is hidden by streaming tokens as they arrive ===");
    List<ChatMessage> messages =
    [
        new(ChatRole.System, SystemPrompt),
        new(ChatRole.User, "In two sentences, what is a statute of limitations?"),
    ];

    Console.Write("Assistant: ");
    await foreach (ChatResponseUpdate update in client.GetStreamingResponseAsync(messages))
    {
        Console.Write(update.Text);
    }

    Console.WriteLine();
}

async Task RunCostAsync(IChatClient client, ILogger log)
{
    log.LogInformation("=== COST demo: watch the 'AI call: ... tokens ... estimatedCostUsd' log line ===");
    ChatResponse response = await client.GetResponseAsync(
    [
        new(ChatRole.System, SystemPrompt),
        new(ChatRole.User, "List three common types of legal documents. One short line each."),
    ]);

    Console.WriteLine(response.Text);
}

async Task RunChaosAsync(IConfiguration config, ILoggerFactory lf, ILogger log)
{
    log.LogInformation("=== CHAOS demo: 1s per-attempt timeout to force failure -> retry -> fallback ===");

    AiSettings settings = config.GetSection(AiSettings.SectionName).Get<AiSettings>() ?? new AiSettings();
    AiCostSettings cost = config.GetSection(AiCostSettings.SectionName).Get<AiCostSettings>() ?? new AiCostSettings();

    // Deliberately break it: a 1s per-attempt timeout against a request large enough to exceed it.
    AiResilienceOptions fragileOptions = new()
    {
        AttemptTimeout = TimeSpan.FromSeconds(1),
        MaxRetryAttempts = 2,
        BaseRetryDelay = TimeSpan.FromMilliseconds(200),
    };

    IChatClient fragile = ServiceCollectionExtensions.BuildChatClient(settings, fragileOptions, cost, lf);

    try
    {
        ChatResponse response = await fragile.GetResponseAsync(
        [
            new(ChatRole.System, SystemPrompt),
            new(ChatRole.User, "Write a detailed 500-word overview of contract law with examples."),
        ]);

        Console.WriteLine(response.Text);
    }
    catch (Exception ex)
    {
        // Graceful degradation: a safe "can't answer" beats a wrong legal answer or a crash.
        log.LogWarning(ex, "All attempts exhausted; returning graceful fallback.");
        ChatResponse fallback = new(new ChatMessage(
            ChatRole.Assistant,
            "I'm unable to answer right now. Please try again shortly."));
        Console.WriteLine("Fallback: " + fallback.Text);
    }
}
