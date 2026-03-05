# LawOffice Infrastructure (Bicep)

Infrastructure-as-Code for the **LawOffice B2C SaaS** demo project. Deploys a complete environment to a single Azure Resource Group.

## Architecture

| Resource | SKU / Tier | Purpose |
|---|---|---|
| Azure Cosmos DB for NoSQL | Serverless | Data store for all microservices |
| Azure Storage Account | Standard LRS | Function Apps runtime storage + blob storage |
| Azure App Service Plan | Consumption (Y1) | Shared plan for all Function Apps |
| 3× Azure Function Apps | Consumption | CaseManagement, OfficeManagement, PartyManagement APIs |
| Azure API Management | Consumption | API gateway with CORS + JWT validation |
| Azure Static Web App | Free | Angular frontend portal |

## Files

| File | Description |
|---|---|
| `main.bicep` | Main template – all environment resources |
| `main.dev.bicepparam` | DEV environment parameter values |
| `main.test.bicepparam` | TEST environment parameter values |
| `modules/cosmos-sql-database.bicep` | Module: Cosmos DB SQL database + containers |
| `policies/apim-global-policy.xml` | APIM global policy template (CORS + optional JWT) |
| `ExportFromAzure.bicep` | Reference-only Azure resource export (not deployed) |

## Cosmos DB Layout

Single Serverless account per environment with three databases mirroring the microservice boundaries:

- **casemanagement** – `cases`, `documentfiles`, `hearings` (partition key: `/officeId`)
- **officemanagement** – `lawyers` (`/officeId`), `offices` (`/id`)
- **partymanagement** – `clients`, `opposingparties` (partition key: `/officeId`)

## Deploy

### Prerequisites

- Azure CLI with Bicep extension (`az bicep upgrade`)
- A resource group – create one if needed:

```bash
az group create --name rg-lawoffice-test --location westeurope
```

### Validate (what-if)

```bash
az deployment group what-if \
  --resource-group rg-lawoffice-test \
  --template-file infra/main.bicep \
  --parameters infra/main.test.bicepparam
```

### Deploy

```bash
az deployment group create \
  --resource-group rg-lawoffice-test \
  --template-file infra/main.bicep \
  --parameters infra/main.test.bicepparam
```

### Post-deployment steps

1. **Deploy Function App code** – use `func azure functionapp publish` or GitHub Actions.
2. **Wire up APIM backends** – redeploy with `configureApimBackends = true` (add it to the `.bicepparam` file or pass via CLI). This calls `listKeys` on each Function App to configure APIM backends with host keys. It requires the Functions host runtime to be running, which only happens after code is published.
3. **Configure Entra ID CIAM** (if needed) – update `jwtOpenIdConfigUrl`, `jwtAudience`, and `jwtIssuer` in the `.bicepparam` file, then redeploy.

> **APIM operations are managed in Bicep:** all Function HTTP triggers are declared as `service/apis/operations` resources in `main.bicep`, so no manual *Import Function App* step is required after deployments.

> **Note:** On first deploy to a new environment, leave `configureApimBackends` as `false` (the default). The APIM named values and backends depend on Function App host keys, which are unavailable until code is deployed. After publishing your Function App code, set `configureApimBackends = true` and redeploy.

## Create a new environment

1. Copy `main.test.bicepparam` → `main.<env>.bicepparam`
2. Set `environmentName` and override any resource names if needed
3. Create a resource group and deploy as shown above
