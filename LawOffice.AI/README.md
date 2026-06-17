# LawOffice.AI

Foundation of the AI bootcamp build.

**Day 1** proves the core architect framing in code: **an LLM endpoint is a flaky, slow, expensive,
non-deterministic, versioned external dependency**, so it is wrapped with the same discipline as any
critical external dependency — provider abstraction, timeout, retry, circuit breaker, fallback, and
cost/observability.

**Day 2** adds prompt management at scale and a strict, validated output contract: prompts become
externalised, versioned content (a prompt registry) rather than string literals, and the model's
output is treated as untrusted input that must satisfy a service-boundary contract — structured,
validated, and repaired-or-refused. See [Prompts & the legal assistant](#prompts--the-legal-assistant-day-2).

**Day 3** builds the tenant-isolated retrieval layer that feeds the assistant: structure-aware
chunking → embeddings → a vector store → tenant-scoped retrieval. The store is **Azure Cosmos DB
integrated vector search**, chosen for cost and architecture fit. See
[Retrieval & multi-tenant isolation](#retrieval--multi-tenant-isolation-day-3).

## Projects

| Project | Role |
|---|---|
| `LawOffice.AI` | Class library. A provider-abstracted `IChatClient` (`Microsoft.Extensions.AI`) over Azure OpenAI, composed as a middleware pipeline (Day 1), a prompt registry and contract-validated legal assistant (Day 2), and a tenant-isolated retrieval layer over Cosmos DB vector search (Day 3). |
| `LawOffice.AI.Harness` | Console app that exercises the library: streaming, a force-failure/retry/fallback "chaos" mode, token+cost logging, the grounded legal-assistant demo, and the end-to-end `rag` retrieval demo. |
| `LawOffice.AI.Tests` | xUnit + Shouldly + NSubstitute. Resilience, cost/usage accounting, prompt store/renderer, contract validation, assistant repair/refusal, chunking, and the tenant-isolation proof — all offline, no model calls. |

## Pipeline (outer → inner)

```
ResilientChatClient      timeout (per attempt) + retry (backoff+jitter) + circuit breaker  [Polly v8]
  └─ UsageLoggingChatClient   logs model, tokens-in/out, latency, estimated USD cost
       └─ UseLogging()        Microsoft.Extensions.AI request/response logging
            └─ Azure OpenAI    AzureOpenAIClient.GetChatClient(deployment).AsIChatClient()
```

Resilience sits outermost, so a retried call is metered once (on the successful attempt); retry and
breaker events log separately. Streaming is forwarded without transparent retry on purpose — a
partially consumed stream cannot be safely replayed; recovery is the caller's concern.

## Prompts & the legal assistant (Day 2)

### Prompt registry — prompts as versioned content, not string literals

Prompts live in `Prompts/Templates/*.prompt.md`, **embedded in the assembly** and resolved by
`(name, version)` through `IPromptStore`. Each file is self-describing via a front-matter header:

```
---
name: legal-assistant
version: v1-terse
description: Terse strict-contract legal Q&A. Answers only from supplied context, else refuses.
---
<template body with {{context}} / {{question}} slots>
```

Two versions of the same prompt name ship today — `v1-terse` and `v2-fewshot` — so the same task can
be authored two ways and compared (Day 6 will measure them). Because prompts are keyed off the parsed
header, renaming a file never breaks resolution.

`PromptRenderer` substitutes `{{slots}}`, **throws on any unfilled slot** (a missing slot is a bug, not
a silently-empty prompt), and **neutralises data-fence breakout** inside untrusted values — a first
mitigation for indirect prompt injection (foreshadowing Day 7): a poisoned document cannot "close" its
data fence and have the rest of its text read as instructions.

### Output contract — structured, validated, repaired-or-refused

The assistant asks the model for a strict JSON contract (`LegalAnswer`) and treats the result as
untrusted input at a service boundary:

```jsonc
{
  "answer": "…",                                  // empty when refusing
  "citations": [ { "sourceId": "…", "quote": "…" } ],  // [] when refusing
  "confidence": "Low" | "Medium" | "High",
  "refusedReason": "…" | null                     // why it refused, or null
}
```

Flow in `LegalAssistant.AnswerAsync`:

```
resolve prompt (name+version) → render with delimited context/question
  → GetResponseAsync<LegalAnswer>  (native JSON-schema structured output)
  → validate the contract (LegalAnswerValidator)
  → on violation: one corrective "repair" turn
  → still invalid: return a safe refusal  (never surface unvalidated content)
```

The validator enforces the **semantics** the schema can't: a refusal must be clean (no answer/citations),
a real answer must cite at least one source, and — the anti-hallucination teeth — **every cited
`sourceId` must be one of the context sources actually supplied**. A citation to an unknown id is a hard
violation. The assistant never throws on bad model output; a safe "can't answer" beats a wrong legal
answer.

## Retrieval & multi-tenant isolation (Day 3)

The retrieval layer (namespace `LawOffice.AI.Retrieval`) turns documents into grounding context for the
Day-2 assistant: **chunk → embed → store → retrieve**. Its output is the same `ContextSource` the
assistant already consumes, so retrieval plugs in with no change to the assistant.

```
StructureAwareChunker   split on clause/section/heading boundaries (+ overlap); FixedSizeChunker = the naive baseline
  → IEmbeddingGenerator  Azure OpenAI text-embedding-3-small (1536 dims), via Microsoft.Extensions.AI
       → IVectorStore     CosmosVectorStore (DiskANN) in prod; InMemoryVectorStore by default (zero-cost, no Azure)
            → TenantDocumentRetriever  embeds the query, searches the tenant's vectors, maps hits → ContextSource
```

### Why Cosmos DB vector search

The app already runs **Cosmos serverless** with a proven `/officeId` tenant partition. Storing
embeddings there adds **zero new infrastructure**, costs ~nothing at idle (pay-per-RU), and **reuses the
multi-tenant isolation already implemented across the services** — the architect's answer over standing
up a separate Azure AI Search service (and it keeps this demo cost-effective). One-time ingest of the
sample docs is ~$0.002 of embeddings; queries are negligible.

### Tenant isolation — defence in depth

`officeId` is a **required first parameter** on every `IVectorStore` operation, so a caller cannot search
or write without naming the tenant. `CosmosVectorStore` then enforces it **twice**: the query is scoped
to the tenant **partition key** *and* carries a `WHERE c.officeId = @officeId` filter (the construction is
in the pure, unit-tested `BuildSearchQuery`). `TenantIsolationTests` is the headline proof — a query
scoped to office A returns **zero** of office B's chunks even when B holds the geometrically closest
vector. This is OWASP LLM "Vector and Embedding Weaknesses" mitigated in code.

### Local-dev note

The proving tests use `InMemoryVectorStore`, so **CI and the default `rag` demo need no Azure**. The
Cosmos path requires a vector-capable store — the **vnext Linux Cosmos emulator** or a real serverless
Cosmos account (the classic emulator does not support `VectorDistance`). Set `AiSettings:Retrieval:Provider`
to `Cosmos` and supply `CosmosConnectionString` to use it; `CosmosVectorStore` creates the
`documentchunks` container (cosine / DiskANN on `/embedding`) on first use.

## Configuration

Reuses the repo-wide `AiSettings:*` keys (same as the CaseManagement summarizer). Non-secret defaults
live in `LawOffice.AI.Harness/appsettings.json`; **the endpoint and key are never committed** — supply
them via user-secrets or environment variables:

```bash
cd LawOffice.AI/LawOffice.AI.Harness
dotnet user-secrets set "AiSettings:Endpoint" "https://<resource>.openai.azure.com"
dotnet user-secrets set "AiSettings:ApiKey"   "<key>"
# or: $env:AiSettings__Endpoint / $env:AiSettings__ApiKey
```

Optional tuning sections: `AiSettings:Resilience` (timeout, retries, breaker thresholds),
`AiSettings:Cost` (per-1K-token USD rates per model), `AiSettings:LegalAssistant`
(`PromptName`, `DefaultPromptVersion`, `MaxRepairAttempts`, `UseJsonSchema`), and `AiSettings:Retrieval`
(`Provider` = `InMemory`|`Cosmos`, `CosmosConnectionString`, `DatabaseId`, `ContainerId`, `TopK`).
Retrieval also uses `AiSettings:EmbeddingDeploymentName` / `EmbeddingDimensions`.

## Run

```bash
dotnet build LawOffice.AI/LawOffice.AI.slnx
dotnet test  LawOffice.AI/LawOffice.AI.slnx          # offline, no key needed

dotnet run --project LawOffice.AI/LawOffice.AI.Harness -- stream   # streamed answer + cost line
dotnet run --project LawOffice.AI/LawOffice.AI.Harness -- chaos    # 1s timeout → retries → fallback
dotnet run --project LawOffice.AI/LawOffice.AI.Harness -- cost     # non-streaming; token+cost log
dotnet run --project LawOffice.AI/LawOffice.AI.Harness -- legal    # grounded assistant: v1 vs v2 + a refusal
dotnet run --project LawOffice.AI/LawOffice.AI.Harness -- rag      # chunk → embed → retrieve → grounded, cited answer
```

The `legal` demo runs an answerable question through both prompt versions (eyeball terse vs few-shot),
then asks a question the supplied context does not cover and shows the assistant refuse with a populated
`refusedReason` and no citations.

The `rag` demo ingests the sample legal docs in `LawOffice.AI.Harness/SampleData/`, prints the
structure-aware vs fixed-size chunk comparison, stores embeddings under two tenants, then retrieves
(office A only) and feeds the results into the legal assistant for a grounded, cited answer. It needs an
Azure OpenAI **embedding** deployment (and a chat deployment) configured; it uses the in-memory store by
default, so no Cosmos is required.
