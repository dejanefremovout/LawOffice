# LawOffice

Multi-tenant B2C SaaS platform for small law offices — manage cases, hearings, documents, clients, and office staff through a modern web application.

> **Live demo**: [https://white-mud-0d598d603.6.azurestaticapps.net/](https://white-mud-0d598d603.6.azurestaticapps.net/)
>
> Create a new office account to explore the full feature set — no existing credentials required.

---

## Features

- **Case Management** — Create, track, and close legal cases with metadata (court, judge, year, parties)
- **Hearing Scheduling** — Schedule court hearings linked to cases with courtroom and date tracking
- **Document Management** — Upload and download case documents directly to Azure Blob Storage via SAS URIs
- **Party Management** — Manage clients and opposing parties with contact information
- **Office Administration** — Configure office details and manage lawyer profiles with invitation codes
- **Multi-Tenancy** — Each law office's data is fully isolated via partition-key-based tenant segregation

---

## Architecture

```
┌─────────────┐     ┌───────────────────┐     ┌──────────────────────────┐
│  Angular 21 │────▶│  Azure API Mgmt   │────▶│  Azure Functions (.NET)  │
│  SPA (SWA)  │     │  (Consumption)    │     │  3 microservices         │
└─────────────┘     └───────────────────┘     └────────────┬─────────────┘
       │                     │                              │
       │            JWT validation via              ┌───────┴───────┐
       │            Entra External ID               │               │
       ▼                                            ▼               ▼
┌─────────────┐                           ┌──────────────┐ ┌──────────────┐
│ Blob Storage│                           │  Cosmos DB   │ │  Cosmos DB   │
│ (documents) │                           │  (Serverless)│ │  (3 DBs)     │
└─────────────┘                           └──────────────┘ └──────────────┘
```

| Layer              | Technology                                   |
|--------------------|----------------------------------------------|
| Frontend           | Angular 21, Angular Material, MSAL Angular   |
| API Gateway        | Azure API Management (Consumption)           |
| Backend            | Azure Functions v4, .NET 10 isolated worker  |
| Database           | Azure Cosmos DB NoSQL (Serverless)           |
| File Storage       | Azure Blob Storage (SAS-based upload)        |
| Identity           | Microsoft Entra External ID (CIAM)           |
| Infrastructure     | Bicep (IaC), Docker Compose (local dev)      |

### Microservices

| Service              | Domain                              | Database            | Entities                     |
|----------------------|-------------------------------------|---------------------|------------------------------|
| CaseManagement API   | Cases, hearings, document files     | `casemanagement`    | Case, Hearing, DocumentFile  |
| OfficeManagement API | Office settings, lawyer profiles    | `officemanagement`  | Office, Lawyer               |
| PartyManagement API  | Clients, opposing parties           | `partymanagement`   | Client, OpposingParty        |

Each service follows a **Clean Architecture** pattern: API → Application → Domain → Infrastructure.

---

## Repository Structure

```
LawOffice/
├── CaseManagement/           # Case management microservice (.NET solution)
├── OfficeManagement/         # Office management microservice (.NET solution)
├── PartyManagement/          # Party management microservice (.NET solution)
├── LawOfficePortal/          # Angular SPA frontend
├── LocalDevelopment/         # Cosmos DB seeder for local Docker
├── infra/                    # Bicep IaC templates and APIM policies
├── docs/                     # Documentation
│   ├── architecture/         # Solution Architect documentation
│   └── LOCAL_DOCKER_DEVELOPMENT.md
└── docker-compose.local.yml  # Local development Docker Compose
```

---

## Prerequisites

### Local Development

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)

### Azure Deployment

- [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) with Bicep extension
- [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org/) and npm

---

## Getting Started (Local Development)

The entire platform runs locally in Docker — no Azure subscription required.

### 1. Clone the repository

```bash
git clone https://github.com/dejanefremovout/LawOffice.git
cd LawOffice
```

### 2. Configure environment

```powershell
Copy-Item .env.local.example .env.local
```

Review and adjust values in `.env.local` if needed.

### 3. Start all services

```powershell
docker compose --env-file .env.local -f docker-compose.local.yml up -d --build
```

This starts:

| Service            | URL                         | Description                     |
|--------------------|-----------------------------|---------------------------------|
| Angular Portal     | http://localhost:4200       | Frontend application            |
| OfficeManagement   | http://localhost:7206/api   | Office/lawyer API               |
| PartyManagement    | http://localhost:7207/api   | Client/opposing party API       |
| CaseManagement     | http://localhost:7208/api   | Case/hearing/document API       |
| Cosmos DB Emulator | https://localhost:8081      | Data Explorer (self-signed cert)|
| Azurite            | http://localhost:10000      | Blob storage emulator           |

### 4. Verify services

```powershell
docker compose --env-file .env.local -f docker-compose.local.yml ps
```

### 5. Follow logs

```powershell
docker compose --env-file .env.local -f docker-compose.local.yml logs -f cosmos-seeder azurite-cors case-api office-api party-api portal
```

### 6. Stop

```powershell
# Preserve data
docker compose --env-file .env.local -f docker-compose.local.yml down

# Full reset (removes volumes)
docker compose --env-file .env.local -f docker-compose.local.yml down -v
```

For troubleshooting and advanced local development details, see [docs/LOCAL_DOCKER_DEVELOPMENT.md](docs/LOCAL_DOCKER_DEVELOPMENT.md).

---

## Azure Deployment

### 1. Create a resource group

```bash
az group create --name rg-lawoffice-dev --location westeurope
```

### 2. Deploy infrastructure

```bash
az deployment group create \
  --resource-group rg-lawoffice-dev \
  --template-file infra/main.bicep \
  --parameters infra/main.dev.bicepparam
```

### 3. Publish Function App code

```bash
func azure functionapp publish func-lawoffice-casemanagement-dev
func azure functionapp publish func-lawoffice-officemanagement-dev
func azure functionapp publish func-lawoffice-partymanagement-dev
```

### 4. Wire APIM backends

After code is deployed, redeploy infrastructure with backends enabled:

```bash
az deployment group create \
  --resource-group rg-lawoffice-dev \
  --template-file infra/main.bicep \
  --parameters infra/main.dev.bicepparam \
  --parameters configureApimBackends=true
```

For full IaC details and multi-environment setup, see [infra/README.md](infra/README.md).

---

## Documentation

Comprehensive Solution Architect documentation is available under [`docs/architecture/`](docs/architecture/):

| Document                          | Scope                                              |
|-----------------------------------|----------------------------------------------------|
| [Solution Architecture Overview](docs/architecture/SOLUTION_ARCHITECTURE_OVERVIEW.md) | C4 diagrams, technology stack, architecture principles |
| [Infrastructure & Deployment](docs/architecture/INFRASTRUCTURE_AND_DEPLOYMENT.md)     | Azure resources, Bicep IaC, environments, Docker   |
| [Security Architecture](docs/architecture/SECURITY_ARCHITECTURE.md)                   | Identity, AuthN/AuthZ, tenant isolation, TLS       |
| [API Design](docs/architecture/API_DESIGN.md)                                         | API catalog (33 operations), contracts, models     |
| [Data Architecture](docs/architecture/DATA_ARCHITECTURE.md)                           | Cosmos DB, partitioning, blob storage, consistency |
| [Architecture Decision Records](docs/architecture/ARCHITECTURE_DECISION_RECORDS.md)   | 16 ADRs with rationale and trade-offs              |
| [Operational Runbook](docs/architecture/OPERATIONAL_RUNBOOK.md)                       | Monitoring, incident response, maintenance         |
| [Cost Analysis](docs/architecture/COST_ANALYSIS.md)                                   | Cost model, scaling projections, optimization      |

---

## Tech Stack

| Category         | Technologies                                          |
|------------------|-------------------------------------------------------|
| **Frontend**     | Angular 21, Angular Material 21, TypeScript 5.9       |
| **Auth**         | MSAL Angular 5.1, Entra External ID (CIAM)           |
| **Backend**      | .NET 10, Azure Functions v4 (isolated worker)         |
| **Data**         | Azure Cosmos DB NoSQL, Azure Blob Storage             |
| **Gateway**      | Azure API Management (Consumption)                    |
| **Hosting**      | Azure Static Web Apps (Free)                          |
| **IaC**          | Bicep                                                 |
| **Testing**      | xUnit, NSubstitute, Shouldly, Vitest                  |
| **Local Dev**    | Docker Compose, Cosmos Emulator, Azurite              |

---

## License

See [LICENSE.txt](LICENSE.txt) for details.