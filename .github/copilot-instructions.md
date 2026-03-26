# LawOffice – Copilot Instructions

Portfolio demo — B2C SaaS for small law offices. Multi-tenant platform with three back-end microservices (Azure Functions v4, .NET 10, isolated worker) + one Angular 21 SPA, deployed on Azure. Identity is handled by **Microsoft Entra External ID**. See [docs/architecture/](../docs/architecture/) for ADRs and design docs.

## Architecture

Clean architecture enforced consistently across all services:

```
API  →  Application  →  Domain  ←  Infrastructure
```

- **Domain** owns entities and repository interfaces; no external dependencies.
- **Infrastructure** implements repositories (CosmosDB, Blob, Service Bus).
- **Application** orchestrates use cases via injected domain interfaces.
- **API** wires DI, handles HTTP transport, maps exceptions to status codes.

Services and their primary containers:
| Service | Domain responsibility |
|---|---|
| CaseManagement | Cases, hearings, document blob SAS |
| OfficeManagement | Office profile, lawyers, Entra/Graph sign-up flows |
| PartyManagement | Clients and opposing parties; publishes delete events |

Tenant boundary: every request carries `X-Office-Id` header, which scopes ALL Cosmos queries. Never query across tenants.

Integration: PartyManagement → Service Bus queue `q-opposingparty-deleted` → CaseManagement queue-trigger reconciliation. See [CaseManagement.Api/Functions/OpposingPartyDeletedQueueFunction.cs](../CaseManagement/CaseManagement.Api/Functions/OpposingPartyDeletedQueueFunction.cs).

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
```

### Run a single service locally
```bash
cd <Service>/<Service>.Api && func start
```

### Full local stack
```bash
docker compose -f docker-compose.local.yml up
```
See [docs/LOCAL_DOCKER_DEVELOPMENT.md](../docs/LOCAL_DOCKER_DEVELOPMENT.md) or [docs/LOCAL_K8S_DEVELOPMENT.md](../docs/LOCAL_K8S_DEVELOPMENT.md).

## Conventions

### .NET

- **DI lifetimes**: SDK clients (`CosmosClient`, `BlobServiceClient`, `ServiceBusClient`, `GraphServiceClient`) → **singleton**; repositories and application services → **scoped**.
- **DI wiring**: split into `AddApplicationServices`, `AddCosmosRepositories`, and service-specific extension methods in `<Service>.Api/Extensions/ServiceCollectionExtensions.cs`.
- **Error handling in Functions**: catch `ArgumentException` → 400, generic `Exception` → 500.
- **Authorization level**: `AuthorizationLevel.Function` on all HTTP triggers.
- **Entity invariants**: enforced in constructors and mutating methods; throw `ArgumentException` for invalid state. Never allow default/empty identifiers or dates.
- **Cosmos queries**: always parameterized `QueryDefinition`; always tenant-scoped with `officeId`.
- **Partition key**: `/officeId` across all containers.

### Angular

- MSAL Angular (`@azure/msal-angular`) handles auth; tokens acquired silently in interceptor.
- Component libraries: Angular Material 21.

### Testing (.NET)

- Frameworks: **xUnit + Shouldly + NSubstitute**.
- Three test projects per service: `Api.Tests` (function-level, mock services), `Application.Tests` (service orchestration, mock repos), `Domain.Tests` (entity invariants, no mocks).
- Mirror the source folder structure under the test project.

### Testing (Angular)
- Framework: **Vitest** (via Angular test builder).

## Gotchas

- **Azurite API version**: local Docker uses `--skipApiVersionCheck` to accept newer SDK versions. See [docker-compose.local.yml](../docker-compose.local.yml).
- **Blob SAS public URIs**: configure `BlobSettings:PublicSasBaseUri` to `http://localhost:10000` locally so the browser can reach Azurite.
- **OfficeManagement startup**: requires Graph settings (`GraphApi:TenantId`, `GraphApi:ClientId`, `GraphApi:ClientSecret`). Missing values crash the host.
- **K8s – Cosmos/Azurite DNS**: use short DNS names (e.g. `azurite`, `cosmos`) within the cluster namespace; FQDN with dots causes HTTP 400 from Azurite.
- **K8s – readiness probes**: use `tcpSocket` for Azurite and Azure Functions (`func start` has no `/api/health`).
- **K8s – Angular memory**: needs ≥ 1Gi memory limit to avoid OOMKill.

## Infrastructure

Bicep in [infra/](../infra/). Data-driven: microservices array drives Function Apps, Cosmos databases, and APIM wiring. See [infra/README.md](../infra/README.md).
