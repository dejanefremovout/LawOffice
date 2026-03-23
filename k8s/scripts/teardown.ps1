<#
.SYNOPSIS
    Tears down the LawOffice Kubernetes local development environment.

.DESCRIPTION
    This script provides options to:
    - Delete only the lawoffice namespace (keeps minikube cluster)
    - Delete the entire minikube cluster

.NOTES
    Run from the repository root: .\k8s\scripts\teardown.ps1
#>

param(
    [switch]$DeleteCluster
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " LawOffice K8s Teardown" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($DeleteCluster) {
    Write-Host "Deleting entire minikube cluster..." -ForegroundColor Yellow
    minikube delete
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Minikube cluster deleted." -ForegroundColor Green
    }
    else {
        Write-Host "Failed to delete minikube cluster." -ForegroundColor Red
        exit 1
    }
}
else {
    Write-Host "Deleting lawoffice namespace (keeps minikube cluster running)..." -ForegroundColor Yellow
    kubectl delete namespace lawoffice --ignore-not-found
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Namespace 'lawoffice' deleted. All pods, services, and resources removed." -ForegroundColor Green
        Write-Host ""
        Write-Host "To also delete the minikube cluster:" -ForegroundColor Gray
        Write-Host "  .\k8s\scripts\teardown.ps1 -DeleteCluster" -ForegroundColor White
    }
    else {
        Write-Host "Failed to delete namespace." -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
