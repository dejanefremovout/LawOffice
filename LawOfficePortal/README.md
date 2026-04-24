# LawyerOfficePortal


This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 21.1.3.

## Overview

This is an admin-style Angular portal with a collapsible left menu panel and a main content area on the right. The initial navigation includes:

- Home
- Clients
- Cases
- Office

## Development server

To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run: 

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Vitest](https://vitest.dev/) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## API Client Generation

TypeScript API clients are auto-generated from OpenAPI 3.0.1 specification files using `openapi-typescript-codegen`.

### Spec files

One JSON spec per backend microservice, committed to `openapi-specs/`:

| File                          | Service              |
|-------------------------------|----------------------|
| `case-management.json`        | CaseManagement API   |
| `office-management.json`      | OfficeManagement API |
| `party-management.json`       | PartyManagement API  |

### Regenerate clients

After a backend API change, update the spec file and run:

```bash
npm run generate:api          # regenerate all three clients
npm run generate:api:case     # CaseManagement only
npm run generate:api:office   # OfficeManagement only
npm run generate:api:party    # PartyManagement only
```

Generated code is written to `src/app/api/<service>/` (models, services, core). Wrapper services in `src/app/services/` delegate to the generated clients so that components are not coupled to the generated code directly.

Important: `npm run generate:api:*` only regenerates from the committed JSON files under `openapi-specs/`. It does not fetch the latest schema from a running backend automatically.

Recommended workflow after changing a backend endpoint:

```bash
1. Add or update the OpenAPI attributes in the Function App code.
2. Run the backend locally and verify the endpoint appears in Swagger UI at /api/swagger/ui.
3. Refresh the committed spec file in `openapi-specs/` for that service.
4. Run npm run generate:api:case   # or office / party
```

If the codegen command succeeds but no generated files change, the most likely cause is a stale spec file rather than a generator failure.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
