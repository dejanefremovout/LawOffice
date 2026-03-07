# Local Docker Development

This setup is local-only. Azure deployment remains unchanged and continues using Bicep + direct code deployments.

## What runs in Docker

- Cosmos DB Emulator (shared)
- Azurite Storage Emulator (shared)
- Cosmos seeder (creates DBs and containers)
- 3x Azure Functions APIs
- Angular frontend

## APIM local mode

APIM is not containerized for local development. The frontend calls Function APIs directly on localhost ports:

- OfficeManagement: `http://localhost:7206/api`
- PartyManagement: `http://localhost:7207/api`
- CaseManagement: `http://localhost:7208/api`

## One-time setup

1. Copy `.env.local.example` to `.env.local`
2. Review values in `.env.local`

## Run

```powershell
docker compose --env-file .env.local -f docker-compose.local.yml up -d --build
```

Check status:

```powershell
docker compose --env-file .env.local -f docker-compose.local.yml ps
```

Follow logs:

```powershell
docker compose --env-file .env.local -f docker-compose.local.yml logs -f cosmos-seeder azurite-cors case-api office-api party-api portal
```

Stop:

```powershell
docker compose --env-file .env.local -f docker-compose.local.yml down
```

## Notes

- `CosmosSettings:ConnectionString` and `BlobSettings:ConnectionString` are injected via compose environment variables, so no code changes are required for Azure.
- `UseDevelopmentStorage=true` is not used inside containers because each container has its own localhost namespace.
- `BlobSettings:ConnectionString` is set for `CaseManagement` only.
- `BlobSettings:PublicSasBaseUri` should be set to `http://localhost:10000` in local Docker so SAS links returned to the browser are reachable outside the Docker network.
- Azurite blob CORS is initialized by the `azurite-cors` one-shot container and should allow direct browser upload from `BLOB_CORS_ALLOWED_ORIGIN` (default `http://localhost:4200`).
- API containers run through Azure Functions Core Tools (`func start`) so the Functions host can provide worker settings such as `Functions:Worker:HostEndpoint`.
- APIs wait for `cosmos-seeder` to complete successfully before startup.
- `cosmos-seeder` retries emulator readiness and transient control-plane errors.
- Cosmos emulator persistence is enabled (`AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE=true`).
- Seeder ensures these resources exist:
  - `casemanagement`: `cases`, `documentfiles`, `hearings`
  - `officemanagement`: `lawyers`, `offices`
  - `partymanagement`: `clients`, `opposingparties`

## If Cosmos has partial/missing containers

Reset local emulator state once and start again:

```powershell
docker compose --env-file .env.local -f docker-compose.local.yml down -v
docker compose --env-file .env.local -f docker-compose.local.yml up -d --build
```

Check seeder logs:

```powershell
docker compose --env-file .env.local -f docker-compose.local.yml logs -f cosmos-seeder
```

If you see `ServiceUnavailable (503)` with messages like `high demand in this region South Central US`, treat it as a transient emulator capacity/startup issue in local Docker, not a real Azure region quota for your subscription.

If you suspect stale emulator state, always run `down -v` before the next `up --build`.

## If browser upload fails with CORS

1. Ensure `.env.local` contains `BLOB_CORS_ALLOWED_ORIGIN=http://localhost:4200`.
2. Recreate containers so `azurite-cors` runs again:

```powershell
docker compose --env-file .env.local -f docker-compose.local.yml down
docker compose --env-file .env.local -f docker-compose.local.yml up -d --build
```

3. Check the CORS init logs:

```powershell
docker compose --env-file .env.local -f docker-compose.local.yml logs azurite-cors
```
