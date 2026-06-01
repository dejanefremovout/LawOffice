# LawOffice.AI

Day-1 foundation of the AI bootcamp build. Proves the core architect framing in code:
**an LLM endpoint is a flaky, slow, expensive, non-deterministic, versioned external dependency**,
so it is wrapped with the same discipline as any critical external dependency — provider
abstraction, timeout, retry, circuit breaker, fallback, and cost/observability.

## Projects

| Project | Role |
|---|---|
| `LawOffice.AI` | Class library. A provider-abstracted `IChatClient` (`Microsoft.Extensions.AI`) over Azure OpenAI, composed as a middleware pipeline: **resilience → usage/cost logging → standard logging → Azure OpenAI**. |
| `LawOffice.AI.Harness` | Console app that exercises the library: streaming, a force-failure/retry/fallback "chaos" mode, and token+cost logging. |
| `LawOffice.AI.Tests` | xUnit + Shouldly + NSubstitute. Resilience (retry / circuit-breaker / no-retry-on-cancel) and cost/usage accounting — all offline, no model calls. |

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

Optional tuning sections: `AiSettings:Resilience` (timeout, retries, breaker thresholds) and
`AiSettings:Cost` (per-1K-token USD rates per model).

## Run

```bash
dotnet build LawOffice.AI/LawOffice.AI.slnx
dotnet test  LawOffice.AI/LawOffice.AI.slnx          # offline, no key needed

dotnet run --project LawOffice.AI/LawOffice.AI.Harness -- stream   # streamed answer + cost line
dotnet run --project LawOffice.AI/LawOffice.AI.Harness -- chaos    # 1s timeout → retries → fallback
dotnet run --project LawOffice.AI/LawOffice.AI.Harness -- cost     # non-streaming; token+cost log
```
