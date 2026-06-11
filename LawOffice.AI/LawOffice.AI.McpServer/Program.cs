using LawOffice.AI.Extensions;
using LawOffice.AI.McpServer;
using LawOffice.AI.Retrieval;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// MCP server exposing two LawOffice tools over stdio:
//   SearchDocuments(query)  -> RAG retrieval over the office's documents (unstructured knowledge)
//   GetCaseStatus(caseId)   -> system-of-record lookup (structured truth)
//
// The session's office is fixed from configuration (McpServer:OfficeId) — the authenticated identity —
// and never taken from a tool argument, so the model cannot reach another office's data.
//
// Endpoint/ApiKey come from user-secrets or environment (AiSettings__Endpoint / AiSettings__ApiKey),
// never from source control.

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true);
builder.Configuration.AddEnvironmentVariables();

// stdio transport owns stdout — it is the JSON-RPC channel. Any log written there corrupts the protocol,
// so route every log to stderr instead.
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

// Reuse the existing LawOfficeAI stack: chat client, embeddings, tenant-scoped vector store, retriever, RAG pipeline.
builder.Services.AddLawOfficeAi(builder.Configuration);

// The session tenant = authenticated identity. Default to "office-a" for the local demo.
string officeId = builder.Configuration["McpServer:OfficeId"] ?? "office-a";
builder.Services.AddSingleton<ITenantContext>(new ConfiguredTenantContext(officeId));
builder.Services.AddSingleton<ICaseStatusProvider>(InMemoryCaseStatusProvider.WithDemoSeed());

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

using IHost host = builder.Build();

await SeedDocumentsAsync(host.Services, officeId);

await host.RunAsync();
return;

// The default in-memory vector store starts empty and lives only for this process, so SearchDocuments has
// nothing to return until we ingest. Mirror the Harness "rag" demo: chunk the sample docs (structure-aware)
// and ingest them under the session's office. Wrapped so a missing embedding credential (or Cosmos outage)
// logs a warning instead of preventing the server — GetCaseStatus still works without embeddings.
static async Task SeedDocumentsAsync(IServiceProvider services, string officeId)
{
    ILogger logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("Seed");

    (string DocId, string DocType, string File)[] docs =
    [
        ("lease-12", "lease", "lease-agreement.md"),
        ("nda-3", "nda", "nda.md"),
    ];

    try
    {
        IDocumentChunker chunker = services.GetRequiredService<IDocumentChunker>();
        TenantDocumentRetriever retriever = services.GetRequiredService<TenantDocumentRetriever>();

        int chunkCount = 0;
        foreach ((string docId, string docType, string file) in docs)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "SampleData", file);
            string text = await File.ReadAllTextAsync(path);
            IReadOnlyList<DocumentChunk> chunks = chunker.Chunk(docId, docType, text);
            await retriever.IngestAsync(officeId, chunks);
            chunkCount += chunks.Count;
        }

        logger.LogInformation(
            "Seeded {ChunkCount} chunks from {DocCount} documents for office '{OfficeId}'.",
            chunkCount, docs.Length, officeId);
    }
    catch (Exception ex)
    {
        logger.LogWarning(
            ex,
            "Document seeding failed (SearchDocuments will return no results). " +
            "Set AiSettings:Endpoint/ApiKey to enable embeddings.");
    }
}

// Exposed so the entry-point assembly has a named type for AddUserSecrets and tests.
public partial class Program;
