# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Portfolio demo — B2C SaaS for small law offices. Multi-tenant platform with three back-end microservices (Azure Functions v4, .NET 10, isolated worker) + one Angular 21 SPA, deployed on Azure. Identity is handled by **Microsoft Entra External ID**. See [docs/architecture/](docs/architecture/) for ADRs and design docs.

## Architecture

Clean architecture enforced consistently across all services:

```
API  →  Application  →  Domain  ←  Infrastructure
```

- **Domain** owns entities and repository interfaces; no external dependencies.
- **Infrastructure** implements repositories (CosmosDB, Blob, Service Bus).
- **Application** orchestrates use cases via injected domain interfaces.
- **API** wires DI, handles HTTP transport, maps exceptions to status codes.

| Service | Domain responsibility |
|---|---|
| CaseManagement | Cases, hearings, document blob SAS, AI summaries |
| OfficeManagement | Office profile, lawyers, Entra/Graph sign-up flows |
| PartyManagement | Clients and opposing parties; publishes delete events |

**Tenant boundary**: every request carries `X-Office-Id` header, which scopes ALL Cosmos queries. Never query across tenants.

**Service-to-service integration**: PartyManagement → Service Bus queue `q-opposingparty-deleted` → CaseManagement queue-trigger reconciliation ([CaseManagement.Api/Functions/OpposingPartyDeletedQueueFunction.cs](CaseManagement/CaseManagement.Api/Functions/OpposingPartyDeletedQueueFunction.cs)).

## Build & Test

### .NET services (run from repo root)
```bash
dotnet build  CaseManagement/CaseManagement.slnx
dotnet test   CaseManagement/CaseManagement.slnx

dotnet build  OfficeManagement/OfficeManagement.slnx
dotnet test   OfficeManagement/OfficeManagement.slnx

dotnet build  PartyManagement/PartyManagement.slnx
dotnet test   PartyManagement/PartyManagement.slnx
```

### Angular portal
```bash
cd LawOfficePortal
npm ci && npm start    # dev server on :4200
npm test               # Vitest unit tests
npm run generate:api   # regenerate API clients from OpenAPI specs
```

### Run a single service locally
```bash
cd <Service>/<Service>.Api && func start
```

Swagger UI available at `/api/swagger/ui` when running locally.

### Full local stack (Docker)
```bash
Copy-Item .env.local.example .env.local  # then fill in values
docker compose --env-file .env.local -f docker-compose.local.yml up -d --build
```

See [docs/LOCAL_DOCKER_DEVELOPMENT.md](docs/LOCAL_DOCKER_DEVELOPMENT.md) or [docs/LOCAL_K8S_DEVELOPMENT.md](docs/LOCAL_K8S_DEVELOPMENT.md).

## Gotchas

- **Azurite API version**: local Docker uses `--skipApiVersionCheck`; configure `BlobSettings:PublicSasBaseUri` to `http://localhost:10000` so the browser can reach Azurite.
- **OfficeManagement startup**: requires `GraphApi:TenantId`, `GraphApi:ClientId`, `GraphApi:ClientSecret`. Missing values crash the host; use placeholder values locally.
- **Service Bus**: NOT emulated locally — both Docker Compose and Kubernetes modes require a real Azure Service Bus connection string.
- **K8s DNS**: use short names (`azurite`, `cosmos`) within the cluster namespace; FQDN with dots causes HTTP 400 from Azurite.
- **K8s readiness probes**: use `tcpSocket` for Azurite and Azure Functions (no `/api/health` endpoint).
- **K8s Angular memory**: needs ≥ 1Gi memory limit to avoid OOMKill.

## Infrastructure

Bicep templates in [infra/](infra/). Data-driven: a microservices array drives Function Apps, Cosmos databases, and APIM wiring. Three parameter files for dev/test/master environments. See [infra/README.md](infra/README.md).

Cosmos DB layout — one serverless account, three databases:
- `casemanagement`: cases, documentfiles, hearings, aiusagequotas
- `officemanagement`: lawyers, offices
- `partymanagement`: clients, opposingparties

@SKILL.md
