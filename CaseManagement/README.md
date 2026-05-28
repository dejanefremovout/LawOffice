# CaseManagement

CaseManagement is the microservice responsible for cases, hearings, document metadata, blob SAS access, and AI-assisted document summarization.

## Responsibilities

- CRUD operations for cases, hearings, and document files
- SAS URI generation for direct browser upload/download to Blob Storage
- Queue-trigger reconciliation when opposing parties are deleted in PartyManagement
- AI summary generation for supported text-based document uploads
- Per-office daily AI usage tracking via the `aiusagequotas` Cosmos container

## AI Document Summary

The service exposes a summary endpoint for uploaded documents:

- `POST /documentFile/{documentFileId}/summary`

Behavior:

- Supports text-based files in v1: `.txt`, `.md`, `.json`, `.csv`, `.xml`
- Accepts `summaryDepth` of `short` or `detailed`
- Enforces per-office daily quotas and max input character limits
- Calls Azure OpenAI using the configured deployment name and inference API version

Required configuration for AI:

- `AiSettings:Endpoint`
- `AiSettings:ApiKey`
- `AiSettings:DeploymentName`
- `AiSettings:ApiVersion` (currently `2024-10-21`)
- `AiSettings:DailyQuotaPerOffice`
- `AiSettings:MaxInputChars`

Local development settings live in `CaseManagement.Api/local.settings.json` and are also passed through Docker via `.env.local`.
