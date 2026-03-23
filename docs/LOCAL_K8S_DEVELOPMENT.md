# Local Kubernetes Development

This setup runs the entire LawOffice platform locally on Kubernetes using **minikube** with the Docker driver. It reuses the existing Dockerfiles and mirrors the Docker Compose local setup.

## Architecture Overview

```
┌──────────────────────────────────────────────────────────────┐
│  minikube cluster (Docker driver)                            │
│  namespace: lawoffice                                        │
│                                                              │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────────┐  │
│  │office-api│  │party-api │  │case-api  │  │   portal     │  │
│  │Deployment│  │Deployment│  │Deployment│  │  Deployment  │  │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └──────┬───────┘  │
│       │             │             │               │          │
│  ┌────┴─────────────┴─────────────┴───────────────┴─────┐    │
│  │              NGINX Ingress Controller                │    │
│  │         (minikube addon - path-based routing)        │    │
│  └──────────────────────┬───────────────────────────────┘    │
│                         │                                    │
│  ┌───────────────┐  ┌────────────┐  ┌─────────────────┐      │
│  │ Cosmos DB     │  │  Azurite   │  │  azurite-cors   │      │
│  │ Emulator      │  │  Storage   │  │  (Job)          │      │
│  │ (StatefulSet) │  │(Deployment)│  └─────────────────┘      │
│  └───────────────┘  └────────────┘                           │
└──────────────────────────────────────────────────────────────┘
         │
    minikube tunnel / kubectl port-forward
         │
    localhost (browser)
```

## What runs in Kubernetes

| Component | K8s Resource | Image |
|-----------|-------------|-------|
| Cosmos DB Emulator | StatefulSet + Headless Service | `mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest` |
| Azurite Storage | Deployment + Service | `mcr.microsoft.com/azure-storage/azurite:latest` |
| Azurite CORS init | Job | `mcr.microsoft.com/azure-cli:latest` |
| OfficeManagement API | Deployment + Service | `lawoffice/office-api:local` (built locally) |
| PartyManagement API | Deployment + Service | `lawoffice/party-api:local` (built locally) |
| CaseManagement API | Deployment + Service | `lawoffice/case-api:local` (built locally) |
| Angular Portal | Deployment + Service | `lawoffice/portal:local` (built locally) |
| Routing | Ingress (NGINX) | minikube addon |

## What runs outside Kubernetes

- **Azure Service Bus** — real Azure resource (no first-party emulator), same as Docker Compose setup.

## Prerequisites

1. **Docker Desktop** — must be running
2. **minikube** — install via:
   ```powershell
   winget install Kubernetes.minikube
   ```
3. **kubectl** — install via:
   ```powershell
   winget install Kubernetes.kubectl
   ```

## One-time setup

1. Ensure Docker Desktop is running
2. Ensure `.env.local` exists in the repo root with your `SERVICE_BUS_CONNECTION_STRING` (same file used by Docker Compose)
3. Run the setup script from the repo root:

```powershell
.\k8s\scripts\setup.ps1
```

Or provide the Service Bus connection string directly:

```powershell
.\k8s\scripts\setup.ps1 -ServiceBusConnectionString "Endpoint=sb://..."
```

The script will:
- Start a minikube cluster with the Docker driver (4 CPUs, 6 GB RAM)
- Enable the NGINX Ingress Controller addon
- Build all Docker images inside minikube's Docker daemon
- Apply all Kubernetes manifests (namespace, configmap, secret, deployments, services, ingress)
- Print access instructions

## Accessing the application

### Option A: minikube tunnel (recommended)

Run in a **separate elevated terminal** (requires admin/UAC):

```powershell
minikube tunnel
```

Then access:

| Service | URL |
|---------|-----|
| Portal | http://localhost (via Ingress) |
| OfficeManagement API | http://localhost/office-api/api |
| PartyManagement API | http://localhost/party-api/api |
| CaseManagement API | http://localhost/case-api/api |

### Option B: kubectl port-forward (no admin needed)

Forward individual services to the same ports as Docker Compose:

```powershell
kubectl port-forward -n lawoffice svc/portal 4200:4200
kubectl port-forward -n lawoffice svc/office-api 7206:80
kubectl port-forward -n lawoffice svc/party-api 7207:80
kubectl port-forward -n lawoffice svc/case-api 7208:80
kubectl port-forward -n lawoffice svc/azurite 10000:10000
kubectl port-forward -n lawoffice svc/cosmos 8081:8081
```

Then access:

| Service | URL |
|---------|-----|
| Portal | http://localhost:4200 |
| OfficeManagement API | http://localhost:7206/api |
| PartyManagement API | http://localhost:7207/api |
| CaseManagement API | http://localhost:7208/api |

> **Note:** The portal's runtime config (`config.js`) is set for Ingress mode (Option A) by default. When using port-forward, the portal serves API requests to `/office-api/api` etc., which won't resolve on `localhost:4200`. To use Option B, override the portal ConfigMap:
>
> ```powershell
> kubectl create configmap portal-config -n lawoffice --from-file=config.js=LawOfficePortal/public/config.js --dry-run=client -o yaml | kubectl apply -f -
> kubectl rollout restart deployment/portal -n lawoffice
> ```
>
> This restores the original `config.js` with `localhost:7206/7207/7208` URLs. To switch back to Option A, re-apply the Ingress config:
>
> ```powershell
> kubectl apply -f k8s/manifests/portal/configmap.yaml
> kubectl rollout restart deployment/portal -n lawoffice
> ```

## Useful commands

```powershell
# Pod status
kubectl get pods -n lawoffice

# All resources
kubectl get all -n lawoffice

# Follow logs for a service
kubectl logs -n lawoffice -l app=case-api -f

# Debug a specific pod
kubectl describe pod -n lawoffice <pod-name>

# Shell into a running pod
kubectl exec -it -n lawoffice <pod-name> -- /bin/sh

# View Ingress status
kubectl get ingress -n lawoffice

# Open the Kubernetes dashboard (built-in minikube web UI)
minikube dashboard
```

## Rebuilding after code changes

When you change application code, rebuild and redeploy:

```powershell
# Switch to minikube's Docker daemon
& minikube -p minikube docker-env --shell powershell | Invoke-Expression

# Rebuild the changed image (e.g., case-api)
docker build -t lawoffice/case-api:local -f CaseManagement\CaseManagement.Api\Dockerfile.local .\CaseManagement

# Restart the deployment to pick up the new image
kubectl rollout restart deployment/case-api -n lawoffice

# Watch the rollout
kubectl rollout status deployment/case-api -n lawoffice
```

## Teardown

Delete the lawoffice namespace (keeps minikube cluster for quick restart):

```powershell
.\k8s\scripts\teardown.ps1
```

Delete the entire minikube cluster:

```powershell
.\k8s\scripts\teardown.ps1 -DeleteCluster
```

## Kubernetes concepts demonstrated

| Concept | Where used |
|---------|-----------|
| **Namespace** | All resources in `lawoffice` namespace |
| **Deployment** | APIs, Portal, Azurite |
| **StatefulSet** | Cosmos DB Emulator (persistent state) |
| **Service** (ClusterIP) | Internal networking between pods |
| **Headless Service** | Cosmos DB StatefulSet |
| **Ingress** | NGINX path-based routing (like APIM) |
| **ConfigMap** | Shared configuration (connection strings, settings) |
| **Secret** | Service Bus connection string |
| **Job** | Azurite CORS one-shot initialization |
| **Init Containers** | API pods wait for Cosmos/Azurite readiness |
| **Readiness Probes** | Health checks before routing traffic |
| **Liveness Probes** | Auto-restart unhealthy pods |
| **Resource Requests/Limits** | CPU and memory constraints per container |
| **PersistentVolumeClaim** | Cosmos DB data persistence |
| **Labels** | Resource organization and selection |
| **imagePullPolicy: Never** | Use locally-built images |

## Comparison: Docker Compose vs Kubernetes

| Aspect | Docker Compose | Kubernetes (minikube) |
|--------|---------------|----------------------|
| Startup | `docker compose up` | `.\k8s\scripts\setup.ps1` |
| Orchestration | Docker Compose | kubelet + kube-scheduler |
| Networking | Docker bridge network | Cluster DNS + Services |
| Routing | Direct port mapping | Ingress Controller |
| Configuration | `.env.local` + compose env | ConfigMap + Secret |
| Health checks | None | Readiness + Liveness probes |
| Startup ordering | `depends_on` | Init Containers |
| One-shot tasks | `restart: "no"` service | Job |
| Persistent storage | Docker volumes | PersistentVolumeClaim |
| Scaling | `replicas` (manual) | `kubectl scale` / HPA |
| Dashboard | Docker Desktop | `minikube dashboard` |

## Notes

- The Cosmos DB Emulator requires significant resources (~2-4 GB RAM). The minikube cluster is configured with 6 GB RAM to accommodate it alongside the other services.
- Images are built inside minikube's Docker daemon using `minikube docker-env`. This avoids the need for a container registry.
- `imagePullPolicy: Never` ensures Kubernetes uses the locally-built images instead of trying to pull from a registry.
- Init containers provide a more reliable startup ordering mechanism than Docker Compose `depends_on`, because they actually verify the dependency is healthy (not just started).
- The Ingress resource demonstrates the same path-based routing pattern that APIM provides in production, but using the NGINX Ingress Controller.
- Service Bus is not emulated locally — the same real Azure Service Bus namespace is used as with the Docker Compose setup.
