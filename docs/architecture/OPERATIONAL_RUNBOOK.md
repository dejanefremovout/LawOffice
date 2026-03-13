# Operational Runbook

## Document Information

| Item               | Detail                                         |
|--------------------|-------------------------------------------------|
| **Project**        | LawOffice - B2C SaaS for Small Law Offices      |
| **Version**        | 1.0                                              |
| **Last Updated**   | 2026-03-10                                       |

---

## 1. Operational Overview

This runbook covers day-to-day operations, monitoring, incident response, and maintenance procedures for the LawOffice platform. All services are serverless and platform-managed, minimizing operational overhead.

### 1.1 Service Inventory

| Service                          | Type            | Region         | Scaling        |
|----------------------------------|-----------------|----------------|----------------|
| `func-lawoffice-casemanagement-{env}` | Function App | West Europe* | Auto (0-200)  |
| `func-lawoffice-officemanagement-{env}` | Function App | West Europe* | Auto (0-200) |
| `func-lawoffice-partymanagement-{env}` | Function App | West Europe* | Auto (0-200) |
| `apim-lawoffice-{env}`           | APIM           | West Europe*   | Consumption    |
| `swa-lawoffice-portal-{env}`     | Static Web App | Global CDN     | Managed        |
| `cos-lawoffice-officemanagement-{env}` | Cosmos DB  | West Europe*  | Serverless     |
| `stlawoffice{env}shared`         | Storage        | West Europe*   | Standard LRS   |

*Region is parameterized via resource group location.

---

## 2. Monitoring Strategy

### 2.1 Recommended Monitoring Stack

| Tool                         | Purpose                                        |
|------------------------------|------------------------------------------------|
| **Azure Monitor**            | Platform-level metrics and alerting            |
| **Application Insights**     | APM for Function Apps (traces, dependencies)   |
| **Log Analytics Workspace**  | Centralized log aggregation and querying       |
| **Azure Monitor Alerts**     | Proactive alerting on thresholds               |
| **APIM Analytics**           | API usage, latency, error rates                |
| **Cosmos DB Insights**       | RU consumption, partition analytics            |

### 2.2 Key Metrics to Monitor

#### Function Apps

| Metric                        | Threshold             | Severity | Action                          |
|-------------------------------|-----------------------|----------|---------------------------------|
| HTTP 5xx rate                 | > 5% of requests      | Critical | Investigate Function App logs   |
| HTTP 4xx rate                 | > 20% of requests     | Warning  | Check client integration        |
| Execution duration (P95)      | > 10 seconds          | Warning  | Profile and optimize queries    |
| Function execution count      | Anomaly detection     | Info     | Monitor for abuse               |

#### API Management

| Metric                        | Threshold             | Severity | Action                          |
|-------------------------------|-----------------------|----------|---------------------------------|
| Failed requests               | > 10/minute           | Warning  | Check backend health            |
| Gateway response time (P95)   | > 5 seconds           | Warning  | Check cold starts, backend      |
| Unauthorized (401) rate       | > 50/minute           | Warning  | Potential auth misconfiguration |
| Capacity utilization          | > 80%                 | Critical | Consider upgrade to Standard    |

#### Cosmos DB

| Metric                        | Threshold             | Severity | Action                          |
|-------------------------------|-----------------------|----------|---------------------------------|
| Total RU consumption          | > 3,000 RU/s sustained| Warning  | Approaching 4,000 RU/s cap     |
| Throttled requests (429s)     | Any                   | Critical | RU limit reached; increase cap  |
| Data storage                  | > 75% of limit        | Warning  | Monitor partition growth         |
| Normalized RU consumption     | > 80% per partition   | Warning  | Hot partition detected           |

#### Blob Storage

| Metric                        | Threshold             | Severity | Action                          |
|-------------------------------|-----------------------|----------|---------------------------------|
| Availability                  | < 99.9%               | Critical | Check Azure status page          |
| E2E latency (P95)            | > 500ms               | Warning  | Check network / region           |
| Error rate                    | > 1%                  | Warning  | Check SAS token validity         |

---

## 3. Common Operations

### 3.1 Deploy Infrastructure

```bash
# Validate template
az deployment group validate \
  --resource-group rg-lawoffice-{env} \
  --template-file infra/main.bicep \
  --parameters infra/main.{env}.bicepparam

# Deploy (what-if preview)
az deployment group what-if \
  --resource-group rg-lawoffice-{env} \
  --template-file infra/main.bicep \
  --parameters infra/main.{env}.bicepparam

# Deploy (apply)
az deployment group create \
  --resource-group rg-lawoffice-{env} \
  --template-file infra/main.bicep \
  --parameters infra/main.{env}.bicepparam
```

### 3.2 Deploy Function App Code

```bash
# Build and publish (per microservice)
cd CaseManagement/CaseManagement.Api
func azure functionapp publish func-lawoffice-casemanagement-{env}

cd OfficeManagement/OfficeManagement.Api
func azure functionapp publish func-lawoffice-officemanagement-{env}

cd PartyManagement/PartyManagement.Api
func azure functionapp publish func-lawoffice-partymanagement-{env}
```

### 3.3 Wire APIM Backends (Post Code Deployment)

After deploying Function code for the first time:

```bash
az deployment group create \
  --resource-group rg-lawoffice-{env} \
  --template-file infra/main.bicep \
  --parameters infra/main.{env}.bicepparam \
  --parameters configureApimBackends=true
```

### 3.4 View Function App Logs

```bash
# Stream live logs
func azure functionapp logstream func-lawoffice-casemanagement-{env}

# Query via Azure CLI
az monitor app-insights query \
  --app func-lawoffice-casemanagement-{env} \
  --analytics-query "traces | where timestamp > ago(1h) | order by timestamp desc | take 50"
```

### 3.5 Check Cosmos DB Health

```bash
# List databases
az cosmosdb sql database list \
  --account-name cos-lawoffice-officemanagement-{env} \
  --resource-group rg-lawoffice-{env}

# Check throughput usage
az cosmosdb sql database show \
  --account-name cos-lawoffice-officemanagement-{env} \
  --name casemanagement \
  --resource-group rg-lawoffice-{env}
```

### 3.6 Rotate Function Keys

```bash
# Regenerate host key
az functionapp keys set \
  --name func-lawoffice-casemanagement-{env} \
  --resource-group rg-lawoffice-{env} \
  --key-name default \
  --key-type functionKeys

# After key rotation: redeploy Bicep to update APIM named values
az deployment group create \
  --resource-group rg-lawoffice-{env} \
  --template-file infra/main.bicep \
  --parameters infra/main.{env}.bicepparam
```

---

## 4. Incident Response

### 4.1 Severity Levels

| Level    | Definition                                          | Response Time | Example                            |
|----------|-----------------------------------------------------|---------------|------------------------------------|
| **SEV-1** | Platform down; all users affected                  | Immediate     | All APIs returning 5xx             |
| **SEV-2** | Major feature broken; many users affected          | < 1 hour      | One microservice down              |
| **SEV-3** | Minor feature degraded; some users affected        | < 4 hours     | Slow queries, intermittent errors  |
| **SEV-4** | Cosmetic or low-impact issue                       | Next business day | UI rendering issue              |

### 4.2 Runbook: All APIs Returning 5xx

1. **Check Azure Status**: https://status.azure.com
2. **Check Function App health**: Azure Portal → Function App → Overview → Diagnose and solve problems
3. **Check App Settings**: Verify Cosmos and Storage connection strings are valid
4. **Check Cosmos DB**: Portal → Cosmos Account → Data Explorer → run a simple query
5. **Check Storage Account**: Portal → Storage Account → Access keys → verify connectivity
6. **Check APIM**: Portal → APIM → APIs → Test → run a simple GET
7. **Review logs**: Application Insights → Failures → drill into exceptions

### 4.3 Runbook: High Cosmos DB RU Consumption

1. **Check throttled requests** in Cosmos DB Insights
2. **Identify hot partition** via partition metrics
3. **Identify expensive queries** via Application Insights dependency tracking
4. **Optimize queries** (add filters, reduce returned fields)
5. **If limit reached**: Increase `capacity.totalThroughputLimit` in Bicep and redeploy

### 4.4 Runbook: APIM Cold Start Issues

1. **Confirm cold start** (initial request latency high, subsequent requests normal)
2. **Check deployment**: Ensure Functions have `WEBSITE_RUN_FROM_PACKAGE = 1`
3. **Consider**: Upgrading to APIM Standard v2 or adding a health probe ping

### 4.5 Runbook: JWT Validation Failures (401s)

1. **Check Entra ID service status**
2. **Verify OpenID config URL** is accessible: `curl {jwtOpenIdConfigUrl}`
3. **Verify audience** matches client ID in Entra registration
4. **Verify issuer** matches the CIAM tenant
5. **Check token expiry**: Client may need to refresh MSAL tokens
6. **Check CORS**: Browser may not be sending the Authorization header

---

## 5. Backup & Recovery

### 5.1 Cosmos DB Recovery

| Scenario                    | Recovery Method                                  | RTO          |
|-----------------------------|--------------------------------------------------|--------------|
| Accidental data deletion    | Azure Support ticket → restore from periodic backup | Hours       |
| Entire database corruption  | Azure Support ticket → restore to new account    | Hours        |
| Container dropped           | Re-create via Bicep + restore data from backup   | Hours        |

**Note**: Current periodic backup (4h interval, 8h retention) means maximum data loss of ~4 hours. For production, enable continuous backup.

### 5.2 Blob Storage Recovery

| Scenario                    | Recovery Method                                  | RTO          |
|-----------------------------|--------------------------------------------------|--------------|
| Accidental blob deletion    | Recover from soft delete (7-day window)          | Minutes      |
| Container deletion          | Recover from container soft delete (7-day)       | Minutes      |
| Storage account deletion    | Not recoverable (prevent with resource locks)    | N/A          |

### 5.3 Infrastructure Recovery

| Scenario                    | Recovery Method                                  | RTO          |
|-----------------------------|--------------------------------------------------|--------------|
| Resource misconfiguration   | Redeploy Bicep template (idempotent)            | Minutes      |
| Resource group deletion     | Full redeploy from Bicep + data restore          | Hours        |
| Full environment rebuild    | New RG + Bicep deploy + code publish             | Hours        |

---

## 6. Local Development Operations

### 6.1 Start Local Environment

```bash
# Start all services
docker compose --env-file .env.local -f docker-compose.local.yml up -d --build

# Check service health
docker compose -f docker-compose.local.yml ps

# View logs for a specific service
docker compose -f docker-compose.local.yml logs -f case-api
```

### 6.2 Stop Local Environment

```bash
# Stop all services (preserve data)
docker compose -f docker-compose.local.yml down

# Stop and remove volumes (clean start)
docker compose -f docker-compose.local.yml down -v
```

### 6.3 Common Local Issues

| Issue                          | Cause                          | Resolution                           |
|--------------------------------|--------------------------------|--------------------------------------|
| Cosmos emulator slow startup   | First start / certificate gen  | Wait 2-5 minutes for health          |
| APIs fail on startup           | Seeder not complete            | Check cosmos-seeder logs             |
| SAS URI blob upload fails      | CORS not configured            | Check azurite-cors service completed |
| SAS URI 403 in browser         | Wrong base URI in config       | Set `BLOB_PUBLIC_SAS_BASE_URI=http://localhost:10000` |
| Cosmos SSL error               | Emulator self-signed cert      | Ensure `COSMOS_DISABLE_SSL_VALIDATION=true` |

---

## 7. Maintenance Tasks

### 7.1 Dependency Updates

| Component              | Update Frequency | Process                              |
|------------------------|------------------|--------------------------------------|
| Angular + Material     | Quarterly        | `ng update` + regression testing     |
| .NET SDK               | With LTS releases| Update `.csproj` + build/test       |
| MSAL libraries         | Quarterly        | `npm update` + auth flow testing     |
| NuGet packages         | Monthly          | `dotnet outdated` + compatibility check |
| Bicep API versions     | Bi-annually      | Review and update resource API versions |
| Docker base images     | Monthly          | Update Dockerfile FROM tags          |

### 7.2 Certificate Management

| Certificate              | Managed By        | Rotation                             |
|--------------------------|-------------------|--------------------------------------|
| SWA TLS certificate      | Azure (automatic) | Platform-managed                     |
| Function App TLS         | Azure (automatic) | Platform-managed                     |
| APIM gateway TLS         | Azure (automatic) | Platform-managed                     |
| Cosmos DB TLS            | Azure (automatic) | Platform-managed                     |

### 7.3 Key Rotation Schedule

| Secret                    | Storage           | Rotation Frequency | Process                  |
|---------------------------|-------------------|--------------------|--------------------------|
| Storage Account keys      | Bicep (listed)    | Quarterly          | Azure Portal → Regenerate|
| Cosmos DB keys            | Bicep (listed)    | Quarterly          | Azure Portal → Regenerate|
| Function host keys        | APIM Named Values | Quarterly          | Rotate + redeploy Bicep  |
| Entra client secret       | Entra Portal      | Per Entra policy   | Entra Portal → New secret|

---

## 8. Health Check Endpoints

Currently, no dedicated health check endpoints exist. The following can serve as rudimentary health checks:

| Service              | Health Check                      | Expected Response |
|----------------------|-----------------------------------|-------------------|
| CaseManagement API   | `GET /api/cases/count`            | 200 + JSON body   |
| OfficeManagement API | `GET /api/office`                 | 200 + JSON body   |
| PartyManagement API  | `GET /api/party/count`            | 200 + JSON body   |
| APIM Gateway         | `GET /case/api/cases/count`       | 200 + JSON body   |
| SWA Portal           | `GET /`                           | 200 + HTML        |

**Recommendation**: Add dedicated `/health` endpoints that check downstream dependencies (Cosmos DB, Storage) without requiring authentication.
