# Architecture Decision Records (ADRs)

## Document Information

| Item               | Detail                                         |
|--------------------|-------------------------------------------------|
| **Project**        | LawOffice - B2C SaaS for Small Law Offices      |
| **Version**        | 1.0                                              |
| **Last Updated**   | 2026-03-23                                       |

---

## ADR Index

| ADR   | Title                                              | Status   |
|-------|----------------------------------------------------|----------|
| ADR-001 | Use Microservices Architecture                   | Accepted |
| ADR-002 | Azure Functions on Consumption Plan              | Accepted |
| ADR-003 | Cosmos DB NoSQL as Primary Data Store            | Accepted |
| ADR-004 | Cosmos DB Serverless Capacity Mode               | Accepted |
| ADR-005 | Database-per-Service Data Ownership              | Accepted |
| ADR-006 | Partition Key Strategy (/officeId)               | Accepted |
| ADR-007 | APIM Consumption Tier as API Gateway             | Accepted |
| ADR-008 | Microsoft Entra External ID for Identity         | Accepted |
| ADR-009 | Claim-Based Multi-Tenancy                        | Accepted |
| ADR-010 | Angular SPA on Azure Static Web Apps             | Accepted |
| ADR-011 | Clean Layered Architecture per Service           | Accepted |
| ADR-012 | Bicep for Infrastructure as Code                 | Accepted |
| ADR-013 | Docker Compose for Local Development             | Accepted |
| ADR-014 | SAS URI-Based Document Upload                    | Accepted |
| ADR-015 | Frontend Orchestration for Cross-Service Joins   | Accepted |
| ADR-016 | .NET Isolated Worker Model                       | Accepted |
| ADR-017 | Azure Service Bus Basic for Integration Events   | Accepted |
| ADR-018 | Minikube for Local Kubernetes Development         | Accepted |

---

## ADR-001: Use Microservices Architecture

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

The platform needs to manage multiple distinct business domains (cases, parties, offices) with the potential for independent scaling, deployment, and team ownership.

### Decision

Decompose the system into three microservices aligned to bounded contexts:
- **CaseManagement** - Cases, hearings, document files
- **OfficeManagement** - Offices, lawyers
- **PartyManagement** - Clients, opposing parties

### Consequences

- **Positive**: Independent deployment and scaling per domain; clear ownership boundaries; technology flexibility per service
- **Positive**: Isolated failures - one service can fail without bringing down others
- **Negative**: Cross-service data references require frontend orchestration (no joins)
- **Negative**: Operational complexity increases (3 deployments, 3 databases)

### Alternatives Considered

| Alternative          | Reason Rejected                                            |
|----------------------|------------------------------------------------------------|
| Monolithic API       | Less suitable for demonstrating distributed architecture patterns |
| Two services         | Three bounded contexts naturally emerged from domain modeling |

---

## ADR-002: Azure Functions on Consumption Plan

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

The APIs need a serverless compute platform that minimizes cost for a portfolio project with sporadic traffic while supporting the microservice architecture.

### Decision

Use Azure Functions v4 on the Consumption (Y1 Dynamic) plan for all three microservices.

### Consequences

- **Positive**: Near-zero cost when idle; pay only per execution
- **Positive**: Automatic scaling to handle burst load
- **Positive**: Shared App Service Plan across all 3 Function Apps
- **Negative**: Cold start latency (especially on Consumption tier)
- **Negative**: 10-minute execution timeout limit
- **Negative**: No VNet integration (requires Premium plan)

### Alternatives Considered

| Alternative               | Reason Rejected                                         |
|---------------------------|---------------------------------------------------------|
| App Service (Basic/Standard) | Higher base cost for portfolio workloads             |
| Container Apps            | More operational complexity than needed                 |
| Azure Functions Premium   | Higher base cost; VNet not required for demo            |

---

## ADR-003: Cosmos DB NoSQL as Primary Data Store

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

The application needs a flexible, scalable, multi-tenant database that supports schemaless documents and integrates well with Azure serverless services.

### Decision

Use Azure Cosmos DB for NoSQL (SQL API) as the primary data store for all microservices.

### Consequences

- **Positive**: Flexible schema accommodates evolving domain models
- **Positive**: Built-in partitioning supports multi-tenancy naturally
- **Positive**: SQL-like query language familiar to most developers
- **Positive**: Serverless mode available for cost optimization
- **Positive**: Global distribution ready when scaling internationally
- **Negative**: No cross-partition joins (mitigated by partition key design)
- **Negative**: Cost can escalate unpredictably with poor partition design
- **Negative**: Eventual consistency model requires careful design

### Alternatives Considered

| Alternative        | Reason Rejected                                           |
|--------------------|-----------------------------------------------------------|
| Azure SQL Database | Rigid schema less suited to evolving microservice models  |
| PostgreSQL Flexible| Does not demonstrate Azure-native NoSQL patterns          |
| MongoDB (Cosmos)   | NoSQL API has better Azure-native integration              |

---

## ADR-004: Cosmos DB Serverless Capacity Mode

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

A portfolio/demo project needs the lowest possible data tier cost while maintaining full Cosmos DB functionality.

### Decision

Use Cosmos DB Serverless capacity mode with a 4,000 RU/s throughput limit.

### Consequences

- **Positive**: No base cost (pay per RU consumed)
- **Positive**: Ideal for sporadic, low-volume workloads
- **Positive**: Throughput cap prevents cost surprises
- **Negative**: Higher per-RU cost vs. provisioned throughput at scale
- **Negative**: No SLA guarantees (99.99% availability not included)
- **Negative**: Max 1 region supported

### Scaling Path

When traffic grows beyond portfolio levels:
1. Switch to **Provisioned Throughput** with autoscale
2. Remove the 4,000 RU/s cap 
3. Enable **multi-region** writes for availability

---

## ADR-005: Database-per-Service Data Ownership

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

Each microservice needs to own its data to maintain service independence and avoid coupling through shared database access.

### Decision

Create a separate Cosmos DB database per microservice (3 databases within 1 account):
- `casemanagement` - owned by CaseManagement API
- `officemanagement` - owned by OfficeManagement API
- `partymanagement` - owned by PartyManagement API

### Consequences

- **Positive**: Clear data ownership boundaries
- **Positive**: Each service can evolve its schema independently
- **Positive**: No accidental cross-service data coupling
- **Negative**: Cross-service data resolution happens at the frontend
- **Negative**: All databases share a single Cosmos account (cost optimization for demo)

---

## ADR-006: Partition Key Strategy (/officeId)

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

The partition key determines query efficiency, data distribution, and multi-tenancy isolation in Cosmos DB.

### Decision

Use `/officeId` as the partition key for all containers except `offices` (which uses `/id`).

### Consequences

- **Positive**: All tenant-scoped queries target a single partition (optimal RU cost)
- **Positive**: Physical data isolation between tenants at the partition level
- **Positive**: No cross-partition queries needed for any application use case
- **Negative**: Hot partitions possible if one tenant generates disproportionate traffic
- **Negative**: Maximum partition size of 20 GB per officeId (sufficient for small offices)

---

## ADR-007: APIM Consumption Tier as API Gateway

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

The microservices need a unified API gateway for routing, authentication, and CORS management.

### Decision

Use Azure API Management on the Consumption tier as the central API gateway.

### Consequences

- **Positive**: Unified entry point for all APIs
- **Positive**: Built-in JWT validation (offloads auth from Functions)
- **Positive**: CORS policy management at gateway level
- **Positive**: Near-zero cost on Consumption tier (1M calls/month free)
- **Negative**: Cold start latency (Consumption tier has no warm instances)
- **Negative**: Limited to 1,000 requests/second burst
- **Negative**: No VNet integration on Consumption tier

### Alternatives Considered

| Alternative              | Reason Rejected                                  |
|--------------------------|--------------------------------------------------|
| Azure Front Door         | Overkill for API routing; higher cost            |
| Direct Function App URLs | No unified gateway; auth/CORS per function       |
| APIM Standard v2         | Higher base cost for portfolio workloads         |

---

## ADR-008: Microsoft Entra External ID for Identity

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

The B2C SaaS platform needs a customer-facing identity solution that supports self-service signup, custom claims, and standard protocols.

### Decision

Use Microsoft Entra External ID (CIAM) for all customer identity management.

### Consequences

- **Positive**: Enterprise-grade identity platform with SLA
- **Positive**: Supports custom claims (e.g., `extension_OfficeId`)
- **Positive**: Standard OpenID Connect / OAuth 2.0 protocols
- **Positive**: Built-in self-service sign-up and sign-in flows
- **Positive**: MSAL library integration for Angular SPA
- **Negative**: External dependency outside of IaC (CIAM tenant managed separately)
- **Negative**: Limited customization compared to custom identity solutions

### Alternatives Considered

| Alternative          | Reason Rejected                                        |
|----------------------|--------------------------------------------------------|
| Azure AD B2C         | Being replaced by Entra External ID                    |
| Auth0                | Non-Microsoft; less Azure-native integration           |
| Custom JWT auth      | Security risk; significant development effort          |

---

## ADR-009: Claim-Based Multi-Tenancy

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

The SaaS platform needs to isolate data between law offices (tenants) without provisioning separate infrastructure per tenant.

### Decision

Implement tenant isolation using a custom JWT claim (`extension_OfficeId`) that flows from identity to data layer:

1. Entra ID issues JWT with `extension_OfficeId`
2. APIM validates and extracts claim to HTTP header
3. Function App reads header for query scoping
4. All data queries filter by `officeId` (= partition key)

### Consequences

- **Positive**: Zero per-tenant infrastructure cost
- **Positive**: Tenant context flows from identity to data layer
- **Positive**: APIM prevents header spoofing (overwrites from JWT)
- **Positive**: Partition key provides physical isolation
- **Negative**: Relies on correct query scoping in every repository
- **Negative**: No per-tenant resource limits (noisy neighbor possible)

---

## ADR-010: Angular SPA on Azure Static Web Apps

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

The frontend needs a modern SPA hosting solution with minimal cost and integrated CI/CD.

### Decision

Use Angular 21 hosted on Azure Static Web Apps (Free tier) with GitHub integration.

### Consequences

- **Positive**: Zero hosting cost (Free tier)
- **Positive**: Built-in CI/CD from GitHub
- **Positive**: Global CDN distribution
- **Positive**: Custom domain + SSL certificate support
- **Positive**: Navigation fallback for SPA routing
- **Negative**: Free tier limited to 2 custom domains, 0.5 GB storage
- **Negative**: No server-side rendering (Free tier limitation)

---

## ADR-011: Clean Layered Architecture per Service

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

Each microservice needs a maintainable internal architecture that separates concerns and supports testability.

### Decision

Apply a consistent 4-layer architecture across all microservices:
- **API** (Functions) → HTTP triggers, request parsing
- **Application** (Services) → Business logic orchestration
- **Domain** (Entities) → Pure domain model with invariants
- **Infrastructure** (Repositories) → Data access implementation

### Consequences

- **Positive**: Consistent pattern across all services
- **Positive**: Domain layer is infrastructure-free and highly testable
- **Positive**: DI-based wiring allows easy mocking in tests
- **Positive**: Clear dependency direction (API → Application → Domain ← Infrastructure)
- **Negative**: More projects per service (4 main + test projects each)
- **Negative**: May be over-engineering for simple CRUD operations

---

## ADR-012: Bicep for Infrastructure as Code

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

All Azure resources should be defined as code for repeatability, auditability, and multi-environment deployment.

### Decision

Use Bicep as the Infrastructure as Code language with parameterized, multi-environment deployments.

### Consequences

- **Positive**: Native Azure IaC language with first-class tooling
- **Positive**: Strongly typed, concise syntax (vs. ARM JSON)
- **Positive**: Parameter files per environment (dev/test/master)
- **Positive**: Module system for reusable components (Cosmos DB)
- **Positive**: Data-driven approach (arrays of microservices → loops)
- **Negative**: Azure-only (not portable to other clouds)

### Alternatives Considered

| Alternative   | Reason Rejected                                        |
|---------------|--------------------------------------------------------|
| ARM Templates | Verbose JSON; Bicep compiles to ARM anyway             |
| Terraform     | Additional tool dependency; less Azure-native          |
| Pulumi        | Additional runtime dependency; less Azure-native       |

---

## ADR-013: Docker Compose for Local Development

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

Developers need a local environment that mirrors the cloud architecture for efficient development and testing without Azure costs.

### Decision

Use Docker Compose to orchestrate all local services: Cosmos Emulator, Azurite, 3 Function Apps, and Angular Portal.

### Consequences

- **Positive**: One-command startup (`docker compose up`)
- **Positive**: Mirrors production architecture locally
- **Positive**: No Azure costs during development
- **Positive**: Consistent environment across developer machines
- **Negative**: Cosmos Emulator is resource-intensive (~2-4 GB RAM)
- **Negative**: Docker Desktop license required for commercial use
- **Negative**: SSL complexities with Cosmos Emulator (disabled for local)

> **See also**: [ADR-018](#adr-018-minikube-for-local-kubernetes-development) adds minikube as an alternative local development mode alongside Docker Compose.

---

## ADR-014: SAS URI-Based Document Upload

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

The platform needs to support document file uploads without routing large files through the API layer.

### Decision

Use the **valet key pattern**: the API generates time-limited SAS URIs, and the SPA uploads/downloads directly to/from Blob Storage.

### Consequences

- **Positive**: No file data passes through API (lower compute cost, no timeout risk)
- **Positive**: Direct browser-to-storage transfer (maximum throughput)
- **Positive**: SAS tokens are time-limited and read/write-specific
- **Positive**: No proxy layer needed
- **Negative**: CORS must be configured on Blob Storage
- **Negative**: Additional complexity in managing SAS URI generation

---

## ADR-015: Frontend Orchestration for Cross-Service Joins

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

Cases reference clients and opposing parties from a different microservice. The frontend needs to display combined data.

### Decision

The Angular SPA performs cross-service data composition by making parallel API calls and joining results client-side.

### Consequences

- **Positive**: No service-to-service coupling
- **Positive**: Simple backend APIs (no complex aggregation needed)
- **Positive**: Frontend can cache and reuse party data across views
- **Negative**: Multiple API calls per page load
- **Negative**: Join logic duplicated in frontend

### Alternatives Considered

| Alternative           | Reason Rejected                                        |
|-----------------------|--------------------------------------------------------|
| BFF (Backend for Frontend) | Additional service to maintain                    |
| Event-sourced read model | Significant complexity increase                     |
| Shared database       | Violates microservice data ownership                   |

---

## ADR-016: .NET Isolated Worker Model

**Status**: Accepted  
**Date**: 2026-01-01  

### Context

Azure Functions needs a hosting model that supports the latest .NET versions and provides full control over the application lifecycle.

### Decision

Use the .NET isolated worker model (out-of-process) for all Azure Functions.

### Consequences

- **Positive**: Supports .NET 10 (latest LTS)
- **Positive**: Full control over dependency injection and middleware
- **Positive**: Process isolation from the Functions host
- **Positive**: Standard ASP.NET Core patterns and middleware
- **Negative**: Slight overhead from inter-process communication
- **Negative**: Some minor differences from in-process model documentation

---

## ADR-017: Azure Service Bus Basic for Integration Events

**Status**: Accepted  
**Date**: 2026-03-22  

### Context

Cross-service references are stored by ID (for example, `Case.opposingPartyIds[]`). When an opposing party is deleted in PartyManagement, stale IDs can remain in CaseManagement unless an asynchronous reconciliation mechanism exists.

### Decision

Introduce **Azure Service Bus (Basic tier)** with a queue-based integration pattern:

1. PartyManagement publishes an `OpposingPartyDeleted` message to queue `q-opposingparty-deleted`
2. CaseManagement consumes the message via a Service Bus-triggered function
3. CaseManagement removes the deleted `opposingPartyId` from all matching cases in the same `officeId`

The Basic tier is intentionally selected to keep costs low for demo workloads. Since Basic does not support topics/subscriptions, a single queue pattern is used.

### Consequences

- **Positive**: Introduces practical event-driven architecture with minimal added cost
- **Positive**: Improves eventual consistency between PartyManagement and CaseManagement
- **Positive**: Keeps service boundaries intact (no synchronous service-to-service dependency)
- **Negative**: Queue-only fan-out model is less flexible than topic/subscription
- **Negative**: Local development requires a real Azure Service Bus namespace (no first-party emulator)

### Alternatives Considered

| Alternative                          | Reason Rejected                                         |
|--------------------------------------|---------------------------------------------------------|
| Service Bus Standard (topics)        | Higher cost for current demo scope                      |
| Synchronous service-to-service call  | Tighter runtime coupling and failure propagation         |
| Frontend-only reconciliation         | Incomplete guarantee; stale references remain possible   |

---

## ADR-018: Minikube for Local Kubernetes Development

**Status**: Accepted  
**Date**: 2026-03-23  

### Context

The project uses Docker Compose for local development (ADR-013), which effectively mirrors the cloud architecture. However, Kubernetes is the industry-standard container orchestration platform, and demonstrating Kubernetes skills is valuable for a portfolio project. The challenge is introducing Kubernetes without adding cloud costs to an already cost-optimized serverless architecture.

### Decision

Introduce **minikube** (with the Docker driver) as an **alternative** local development mode alongside the existing Docker Compose setup:

1. Minikube runs a full Kubernetes cluster inside Docker Desktop (no additional VMs or Hyper-V)
2. Existing Dockerfiles are reused as-is - images are built inside minikube's Docker daemon
3. Kubernetes manifests mirror the Docker Compose topology: Cosmos emulator, Azurite, 3 API Deployments, Portal, and NGINX Ingress
4. Docker Compose remains the primary/simpler local development option
5. No cloud Kubernetes resources are provisioned (zero additional cost)

Minikube was chosen over k3d and kind because:
- It uses the **standard Kubernetes distribution** (not K3s), making the experience fully transferable
- It has the **largest documentation ecosystem** and community support for learning
- Its **addon system** (ingress, dashboard, metrics-server) teaches manual component installation
- The **Docker driver** integrates with the existing Docker Desktop setup without additional VM overhead

### Consequences

- **Positive**: Demonstrates Kubernetes concepts (Deployments, Services, Ingress, ConfigMaps, Secrets, Jobs, Init Containers, health probes, resource limits) with zero cloud cost
- **Positive**: Reuses existing Dockerfiles and container images
- **Positive**: NGINX Ingress Controller mirrors APIM path-based routing pattern
- **Positive**: Init containers provide more reliable startup ordering than Docker Compose `depends_on`
- **Positive**: Kubernetes knowledge transfers directly to AKS, EKS, or GKE
- **Negative**: Higher local resource usage than Docker Compose (~6 GB RAM for minikube cluster)
- **Negative**: Additional tooling to install (minikube, kubectl)
- **Negative**: Two local development modes to maintain (Docker Compose and Kubernetes)

### Alternatives Considered

| Alternative                    | Reason Rejected                                              |
|--------------------------------|--------------------------------------------------------------|
| AKS (Azure Kubernetes Service) | Minimum ~$10-35/month VM cost; contradicts serverless cost model |
| k3d (K3s in Docker)            | Lightweight K3s distro; less transferable learning than standard K8s |
| kind (Kubernetes in Docker)    | Similar to minikube but less documented addon ecosystem       |
| Docker Desktop built-in K8s    | Single-node only; less configurable; tied to Docker Desktop   |
| Replace Docker Compose          | Docker Compose is simpler for quick iteration; both serve different needs |
