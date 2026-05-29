# SKILL.md

Coding conventions for the LawOffice repository.

## .NET Conventions

- **OpenAPI**: All HTTP-trigger Functions annotated with `Microsoft.Azure.Functions.Worker.Extensions.OpenApi` attributes (`OpenApiOperation`, `OpenApiParameter`, `OpenApiRequestBody`, `OpenApiResponseWithBody`).
- **DI lifetimes**: SDK clients (`CosmosClient`, `BlobServiceClient`, `ServiceBusClient`, `GraphServiceClient`) → **singleton**; repositories and application services → **scoped**.
- **DI wiring**: split into `AddApplicationServices`, `AddCosmosRepositories`, and service-specific extension methods in `<Service>.Api/Extensions/ServiceCollectionExtensions.cs`.
- **Error handling**: catch `ArgumentException` → 400, generic `Exception` → 500.
- **Authorization level**: `AuthorizationLevel.Function` on all HTTP triggers.
- **Entity invariants**: enforced in constructors and mutating methods; throw `ArgumentException` for invalid state. Never allow default/empty identifiers or dates.
- **Cosmos queries**: always parameterized `QueryDefinition`; always tenant-scoped with `officeId`. Partition key is `/officeId` across all containers.

## Angular Conventions

- MSAL Angular (`@azure/msal-angular`) handles auth; tokens acquired silently in interceptor.
- Component library: Angular Material 21.
- **API clients**: auto-generated from OpenAPI specs via `openapi-typescript-codegen`. Specs in `LawOfficePortal/openapi-specs/`, generated code in `src/app/api/`. Wrapper services in `src/app/services/` delegate to generated clients.

## Testing

**.NET** — xUnit + Shouldly + NSubstitute. Three test projects per service:
- `*.Api.Tests` — function-level tests, mock services
- `*.Application.Tests` — service orchestration, mock repos
- `*.Domain.Tests` — entity invariants, no mocks

Mirror the source folder structure under each test project.

**Angular** — Vitest (via Angular test builder).
