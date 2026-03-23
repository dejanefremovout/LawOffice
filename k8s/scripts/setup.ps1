<#
.SYNOPSIS
    Sets up the LawOffice Kubernetes local development environment using minikube.

.DESCRIPTION
    This script:
    1. Validates prerequisites (minikube, kubectl, Docker Desktop)
    2. Creates or starts a minikube cluster with the Docker driver
    3. Enables the NGINX Ingress Controller addon
    4. Builds Docker images inside minikube's Docker daemon
    5. Applies all Kubernetes manifests
    6. Starts minikube tunnel for localhost access

.NOTES
    Run from the repository root: .\k8s\scripts\setup.ps1
    Requires: Docker Desktop, minikube, kubectl
#>

param(
    [string]$ServiceBusConnectionString = ""
)

$ErrorActionPreference = "Stop"
$RepoRoot = (Resolve-Path "$PSScriptRoot\..\..").Path
$ManifestsPath = Join-Path $RepoRoot "k8s\manifests"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " LawOffice K8s Local Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ---------- Prerequisites ----------

Write-Host "[1/8] Checking prerequisites..." -ForegroundColor Yellow

$missing = @()
if (-not (Get-Command minikube -ErrorAction SilentlyContinue)) { $missing += "minikube" }
if (-not (Get-Command kubectl -ErrorAction SilentlyContinue)) { $missing += "kubectl" }
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) { $missing += "docker" }

if ($missing.Count -gt 0) {
    Write-Host "Missing required tools: $($missing -join ', ')" -ForegroundColor Red
    Write-Host ""
    Write-Host "Install them:" -ForegroundColor Yellow
    if ($missing -contains "minikube") {
        Write-Host "  winget install Kubernetes.minikube" -ForegroundColor Gray
    }
    if ($missing -contains "kubectl") {
        Write-Host "  winget install Kubernetes.kubectl" -ForegroundColor Gray
    }
    if ($missing -contains "docker") {
        Write-Host "  Install Docker Desktop from https://www.docker.com/products/docker-desktop/" -ForegroundColor Gray
    }
    exit 1
}

Write-Host "  All prerequisites found." -ForegroundColor Green

# ---------- Service Bus Connection String ----------

Write-Host "[2/8] Configuring Service Bus..." -ForegroundColor Yellow

if ([string]::IsNullOrWhiteSpace($ServiceBusConnectionString)) {
    # Try reading from .env.local
    $envLocalPath = Join-Path $RepoRoot ".env.local"
    if (Test-Path $envLocalPath) {
        $envContent = Get-Content $envLocalPath -Raw
        if ($envContent -match 'SERVICE_BUS_CONNECTION_STRING=(.+)') {
            $ServiceBusConnectionString = $Matches[1].Trim()
            Write-Host "  Read Service Bus connection string from .env.local" -ForegroundColor Green
        }
    }
}

if ([string]::IsNullOrWhiteSpace($ServiceBusConnectionString)) {
    Write-Host "  WARNING: No Service Bus connection string provided." -ForegroundColor Yellow
    Write-Host "  Service Bus integration will not work. Provide it via:" -ForegroundColor Yellow
    Write-Host "    .\setup.ps1 -ServiceBusConnectionString 'Endpoint=sb://...'" -ForegroundColor Gray
    Write-Host "    or set it in .env.local" -ForegroundColor Gray
    $sbBase64 = ""
}
else {
    $sbBase64 = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($ServiceBusConnectionString))
}

# ---------- Minikube Cluster ----------

Write-Host "[3/8] Starting minikube cluster..." -ForegroundColor Yellow

$minikubeStatus = minikube status --format='{{.Host}}' 2>&1
if ($minikubeStatus -eq "Running") {
    Write-Host "  Minikube is already running." -ForegroundColor Green
}
else {
    Write-Host "  Creating minikube cluster (Docker driver, 4 CPUs, 6GB RAM)..."
    minikube start --driver=docker --cpus=4 --memory=6144 --kubernetes-version=stable
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  Failed to start minikube." -ForegroundColor Red
        exit 1
    }
    Write-Host "  Minikube started." -ForegroundColor Green
}

# ---------- Enable Ingress ----------

Write-Host "[4/8] Enabling NGINX Ingress Controller addon..." -ForegroundColor Yellow

$addons = minikube addons list 2>&1 | Out-String
if ($addons -match "ingress\s*\|\s*minikube\s*\|\s*enabled") {
    Write-Host "  Ingress addon already enabled." -ForegroundColor Green
}
else {
    minikube addons enable ingress
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  Failed to enable ingress addon." -ForegroundColor Red
        exit 1
    }
    Write-Host "  Ingress addon enabled." -ForegroundColor Green
}

# ---------- Build Images ----------

Write-Host "[5/8] Building Docker images inside minikube..." -ForegroundColor Yellow
Write-Host "  Switching to minikube's Docker daemon..."

# Set minikube Docker environment variables for this process
& minikube -p minikube docker-env --shell powershell | Invoke-Expression

Write-Host "  Building lawoffice/office-api:local..."
docker build -t lawoffice/office-api:local -f "$RepoRoot\OfficeManagement\OfficeManagement.Api\Dockerfile.local" "$RepoRoot\OfficeManagement"
if ($LASTEXITCODE -ne 0) { Write-Host "  Failed to build office-api." -ForegroundColor Red; exit 1 }

Write-Host "  Building lawoffice/party-api:local..."
docker build -t lawoffice/party-api:local -f "$RepoRoot\PartyManagement\PartyManagement.Api\Dockerfile.local" "$RepoRoot\PartyManagement"
if ($LASTEXITCODE -ne 0) { Write-Host "  Failed to build party-api." -ForegroundColor Red; exit 1 }

Write-Host "  Building lawoffice/case-api:local..."
docker build -t lawoffice/case-api:local -f "$RepoRoot\CaseManagement\CaseManagement.Api\Dockerfile.local" "$RepoRoot\CaseManagement"
if ($LASTEXITCODE -ne 0) { Write-Host "  Failed to build case-api." -ForegroundColor Red; exit 1 }

Write-Host "  Building lawoffice/portal:local..."
docker build -t lawoffice/portal:local -f "$RepoRoot\LawOfficePortal\Dockerfile.local" "$RepoRoot\LawOfficePortal"
if ($LASTEXITCODE -ne 0) { Write-Host "  Failed to build portal." -ForegroundColor Red; exit 1 }

Write-Host "  All images built." -ForegroundColor Green

# ---------- Apply Manifests ----------

Write-Host "[6/8] Applying Kubernetes manifests..." -ForegroundColor Yellow

# Namespace first
kubectl apply -f "$ManifestsPath\namespace.yaml"

# ConfigMap and Secret (patch secret with actual value)
kubectl apply -f "$ManifestsPath\configmap.yaml"

# Create/update secret with the actual Service Bus connection string
$secretYaml = Get-Content "$ManifestsPath\secret.yaml" -Raw
$secretYaml = $secretYaml -replace 'SERVICE_BUS_CONNECTION_STRING: ""', "SERVICE_BUS_CONNECTION_STRING: `"$sbBase64`""
$secretYaml | kubectl apply -f -

# Infrastructure services
kubectl apply -f "$ManifestsPath\cosmos\statefulset.yaml"
kubectl apply -f "$ManifestsPath\azurite\deployment.yaml"

Write-Host "  Waiting for Cosmos and Azurite pods to be ready (this takes 1-3 minutes)..."
Write-Host "  (Cosmos emulator is resource-intensive; be patient)" -ForegroundColor Gray

# Temporarily relax error preference for kubectl wait — it writes to stderr on timeout,
# which PowerShell treats as a terminating error under $ErrorActionPreference = "Stop".
$ErrorActionPreference = "Continue"
kubectl wait --namespace lawoffice --for=condition=ready pod -l app=azurite --timeout=180s 2>&1 | Out-Null
$azuriteReady = $LASTEXITCODE -eq 0
$ErrorActionPreference = "Stop"

if (-not $azuriteReady) {
    Write-Host "  Azurite not ready within timeout. Check: kubectl logs -n lawoffice -l app=azurite" -ForegroundColor Yellow
}
else {
    Write-Host "  Azurite is ready." -ForegroundColor Green
}

# CORS Job (depends on Azurite being ready)
# Delete previous job run if it exists (jobs are immutable)
$ErrorActionPreference = "Continue"
kubectl delete job azurite-cors -n lawoffice --ignore-not-found 2>&1 | Out-Null
$ErrorActionPreference = "Stop"
kubectl apply -f "$ManifestsPath\azurite\job-cors.yaml"

# API services
kubectl apply -f "$ManifestsPath\office-api\deployment.yaml"
kubectl apply -f "$ManifestsPath\party-api\deployment.yaml"
kubectl apply -f "$ManifestsPath\case-api\deployment.yaml"

# Portal
kubectl apply -f "$ManifestsPath\portal\configmap.yaml"
kubectl apply -f "$ManifestsPath\portal\deployment.yaml"

# Ingress
kubectl apply -f "$ManifestsPath\ingress.yaml"

Write-Host "  All manifests applied." -ForegroundColor Green

# ---------- Status ----------

Write-Host "[7/8] Cluster status:" -ForegroundColor Yellow
Write-Host ""
kubectl get all -n lawoffice
Write-Host ""

# ---------- Tunnel ----------

Write-Host "[8/8] Access instructions:" -ForegroundColor Yellow
Write-Host ""
Write-Host "  Option A - minikube tunnel (recommended):" -ForegroundColor Cyan
Write-Host "    Run in a separate terminal (requires admin/elevated):" -ForegroundColor Gray
Write-Host "      minikube tunnel" -ForegroundColor White
Write-Host "    Then access:" -ForegroundColor Gray
Write-Host "      Portal:          http://localhost:4200  (via Ingress)" -ForegroundColor White
Write-Host "      OfficeManagement: http://localhost/office-api/api" -ForegroundColor White
Write-Host "      PartyManagement:  http://localhost/party-api/api" -ForegroundColor White
Write-Host "      CaseManagement:   http://localhost/case-api/api" -ForegroundColor White
Write-Host ""
Write-Host "  Option B - kubectl port-forward (no admin needed):" -ForegroundColor Cyan
Write-Host "      kubectl port-forward -n lawoffice svc/portal 4200:4200" -ForegroundColor White
Write-Host "      kubectl port-forward -n lawoffice svc/office-api 7206:80" -ForegroundColor White
Write-Host "      kubectl port-forward -n lawoffice svc/party-api 7207:80" -ForegroundColor White
Write-Host "      kubectl port-forward -n lawoffice svc/case-api 7208:80" -ForegroundColor White
Write-Host "      kubectl port-forward -n lawoffice svc/azurite 10000:10000" -ForegroundColor White
Write-Host ""
Write-Host "  Useful commands:" -ForegroundColor Cyan
Write-Host "      kubectl get pods -n lawoffice                    # Pod status" -ForegroundColor White
Write-Host "      kubectl logs -n lawoffice -l app=case-api -f     # Follow logs" -ForegroundColor White
Write-Host "      kubectl describe pod -n lawoffice <pod-name>     # Debug a pod" -ForegroundColor White
Write-Host "      minikube dashboard                               # Web UI" -ForegroundColor White
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Setup complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
