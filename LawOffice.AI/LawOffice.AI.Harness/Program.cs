using System.Text.Json;
using LawOffice.AI;
using LawOffice.AI.Assistant;
using LawOffice.AI.Extensions;
using LawOffice.AI.Resilience;
using LawOffice.AI.Retrieval;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Proves the "LLM = flaky external dependency" framing in code.
//   dotnet run -- stream   streaming completion to the console
//   dotnet run -- chaos    1s timeout -> retry (logged) -> graceful fallback
//   dotnet run -- cost     non-streaming call; token + cost line is logged
//   dotnet run -- legal    grounded assistant: two prompt versions + a refusal, structured + validated
//   dotnet run -- rag      ingest sample docs -> tenant-scoped retrieval -> grounded, cited answer
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
    case "legal":
        await RunLegalAsync(host.Services.GetRequiredService<ILegalAssistant>(), logger);
        break;
    case "rag":
        await RunRagAsync(host.Services, logger);
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

async Task RunLegalAsync(ILegalAssistant assistant, ILogger log)
{
    log.LogInformation("=== LEGAL demo: externalised/versioned prompts + structured, validated output ===");

    // A few fake context snippets with ids the model must cite. Plan is that these come from
    // tenant-scoped retrieval; today they're supplied directly.
    IReadOnlyList<ContextSource> context =
    [
        new("lease-12", "The tenant shall give 60 days' written notice before terminating this lease."),
        new("lease-12-rent", "Monthly rent is EUR 850, due on the first day of each month."),
        new("nda-3", "Confidential information must not be disclosed for a period of five years."),
    ];

    // Same answerable task, two prompt versions: eyeball the difference (later we measure it properly).
    const string answerable = "How much notice must the tenant give to terminate the lease?";
    foreach (string version in new[] { "v1-terse", "v2-fewshot" })
    {
        Console.WriteLine($"\n--- prompt version: {version} ---");
        LegalAnswer answer = await assistant.AnswerAsync(answerable, context, version);
        Console.WriteLine(JsonSerializer.Serialize(answer, LegalAnswer.SerializerOptions));
    }

    // Refusal demo: the answer isn't in the supplied context, so the assistant must refuse.
    Console.WriteLine("\n--- refusal demo (question not covered by context) ---");
    LegalAnswer refusal = await assistant.AnswerAsync(
        "What is the penalty for late rent payment?", context, "v1-terse");
    Console.WriteLine(JsonSerializer.Serialize(refusal, LegalAnswer.SerializerOptions));
}

async Task RunRagAsync(IServiceProvider services, ILogger log)
{
    log.LogInformation("=== RAG demo: chunk -> embed -> tenant-scoped store -> retrieve -> grounded answer ===");

    // Two sample documents with ids consistent with the 'legal' demo.
    (string DocId, string DocType, string File)[] docs =
    [
        ("lease-12", "lease", "lease-agreement.md"),
        ("nda-3", "nda", "nda.md"),
    ];

    var structureChunker = services.GetRequiredService<IDocumentChunker>();
    var fixedChunker = new FixedSizeChunker(windowChars: 500, overlapChars: 60);

    // 1) Chunking comparison: structure-aware respects clause boundaries; fixed-size cuts blindly.
    foreach ((string docId, string docType, string file) in docs)
    {
        string text = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "SampleData", file));

        IReadOnlyList<DocumentChunk> structured = structureChunker.Chunk(docId, docType, text);
        IReadOnlyList<DocumentChunk> naive = fixedChunker.Chunk(docId, docType, text);

        Console.WriteLine($"\n--- chunking '{docId}': structure-aware={structured.Count} chunks, fixed-size={naive.Count} chunks ---");
        Console.WriteLine("structure-aware sections: " + string.Join(" | ", structured.Select(c => c.Section)));
    }

    // 2) Ingest under two tenants to make the isolation boundary concrete.
    const string officeA = "office-a";
    const string officeB = "office-b";
    var retriever = services.GetRequiredService<TenantDocumentRetriever>();

    foreach ((string docId, string docType, string file) in docs)
    {
        string text = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "SampleData", file));
        IReadOnlyList<DocumentChunk> chunks = structureChunker.Chunk(docId, docType, text);
        await retriever.IngestAsync(officeA, chunks);
    }

    // Office B holds only the NDA — proving retrieval for office A never returns it is the isolation story.
    string ndaText = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "SampleData", "nda.md"));
    await retriever.IngestAsync(officeB, structureChunker.Chunk("nda-b", "nda", ndaText));

    // 3) Retrieve (tenant A only) -> feed into the existing grounded assistant.
    const string question = "How much notice must the tenant give to terminate the lease?";
    IReadOnlyList<ContextSource> retrieved = await retriever.RetrieveAsync(officeA, question, topK: 4);

    Console.WriteLine($"\n--- retrieved for {officeA} (top {retrieved.Count}) ---");
    foreach (ContextSource source in retrieved)
    {
        Console.WriteLine($"  [{source.Id}]");
    }

    var assistant = services.GetRequiredService<ILegalAssistant>();
    LegalAnswer answer = await assistant.AnswerAsync(question, retrieved);
    Console.WriteLine("\n--- grounded answer ---");
    Console.WriteLine(JsonSerializer.Serialize(answer, LegalAnswer.SerializerOptions));
}
