# LawOffice.AI.McpServer — Day 5 (MCP server)

A standalone **Model Context Protocol** server that exposes two LawOffice capabilities as MCP **tools**
over stdio, so an MCP-capable host (Claude Desktop) can call them. It reuses the Day 1–4 stack
(`AddLawOfficeAi`) — chat client, embeddings, tenant-scoped vector store, retriever, RAG pipeline.

## Tools

| Tool | Backed by | Shape | Why |
|---|---|---|---|
| `SearchDocuments(query)` | Day 4 RAG retrieval (`TenantDocumentRetriever.RetrieveCandidatesAsync`) | reranked document chunks | **unstructured knowledge** — answer from the content of contracts/agreements |
| `GetCaseStatus(caseId)` | in-memory stub (`ICaseStatusProvider`) | a single case status | **structured truth** — read the live status from the system of record, don't embed it |

The split is the lesson: *retrieval for unstructured knowledge, tools for structured truth.*

## The security spine — tenant from identity, never from the model

Neither tool takes a `tenantId`/`officeId` parameter. Each resolves its office from the injected
`ITenantContext`, which is fixed at startup from configuration (`McpServer:OfficeId`) — standing in for
the authenticated identity (an OAuth claim in a real HTTP deployment). **The model is given no slot to
name a tenant**, so a malicious or confused prompt — including an indirect prompt injection hidden inside
a retrieved document (Day 7) — cannot reach another office's data. Tenant scoping is then enforced again
at the data layer (`InMemoryVectorStore`/`CosmosVectorStore` partition + `WHERE officeId = …`), i.e.
defence in depth. The tools are read-only and least-privilege.

## Run locally

Supply Azure OpenAI credentials (needed to embed the query and the seed documents):

```powershell
cd LawOffice.AI/LawOffice.AI.McpServer
dotnet user-secrets set "AiSettings:Endpoint" "https://<resource>.openai.azure.com/"
dotnet user-secrets set "AiSettings:ApiKey" "<key>"
dotnet run
```

On startup the server seeds the sample documents (`SampleData/*.md`) under `McpServer:OfficeId`
(default `office-a`) into the in-memory vector store, logs that to **stderr** (stdout is the JSON-RPC
channel), and then blocks on stdio waiting for a client. `Ctrl+C` to stop.

> Without credentials the host still starts and `GetCaseStatus` works; `SearchDocuments` returns nothing
> (a warning is logged). To use persisted vectors instead, set `AiSettings:Retrieval:Provider=Cosmos`
> against a populated container.

## Connect Claude Desktop

Add to `claude_desktop_config.json` (Windows: `%APPDATA%\Claude\claude_desktop_config.json`), then
restart Claude Desktop:

```json
{
  "mcpServers": {
    "lawoffice": {
      "command": "dotnet",
      "args": ["run", "--project", "D:\\Projects\\LawOffice\\LawOffice.AI\\LawOffice.AI.McpServer"],
      "env": {
        "AiSettings__Endpoint": "https://<resource>.openai.azure.com/",
        "AiSettings__ApiKey": "<key>",
        "McpServer__OfficeId": "office-a"
      }
    }
  }
}
```

The two tools then appear in the host. Try: *"What notice must a tenant give to end the lease?"*
(→ `SearchDocuments`) and *"What is the status of CASE-001?"* (→ `GetCaseStatus`). Note the advertised
tool schemas contain only `query` / `caseId` — no tenant parameter.

## Reflection — MCP vs in-process function calling

For the LawOffice assistant alone, **both tools could just be in-process function calls**: we own both
ends, there's no second host, and the protocol adds a network boundary, an auth surface, and another
thing to secure. In-process is the correct, simpler default.

MCP earns its keep when a tool needs to be **reused across multiple AI hosts**, **owned/deployed
independently by another team**, or **exposed to third-party/agentic clients** with a standard contract —
turning M hosts × N tools of bespoke glue into M + N (the USB-C / ODBC / OpenAPI argument).

Of the two, **`GetCaseStatus` is the stronger MCP candidate**: case status is an authoritative,
broadly-useful capability that an IDE assistant, a partner integration, or a future agent would all want,
and it maps cleanly to a stable, independently-owned contract over CaseManagement's
`ICaseService.Get(caseId, officeId)`. **`SearchDocuments`** is more tightly coupled to *our* retrieval
pipeline and tenant model, so it's the kind of capability you'd keep in-process until a concrete second
consumer appears. We expose both here to exercise the protocol, while being honest that today they only
*need* to be in-process.

## Security note (bridge to Day 7)

Every tool is an attack surface and a potential *excessive agency* lever. Mitigations applied here:
identity-derived tenant scoping (never model-supplied), read-only least-privilege tools (no delete/write),
and treating retrieved document text as untrusted input. A production HTTP deployment adds OAuth-based
authorization (the C# MCP SDK v1.x supports protected-resource-metadata discovery), per-tenant rate
limits/token ceilings, and human-in-the-loop for any high-impact action.
