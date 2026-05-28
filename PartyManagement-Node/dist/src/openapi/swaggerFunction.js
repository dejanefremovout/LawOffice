"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getOpenApiSpecHandler = getOpenApiSpecHandler;
exports.getSwaggerUiHandler = getSwaggerUiHandler;
const functions_1 = require("@azure/functions");
const specBuilder_1 = require("./specBuilder");
const specJson = JSON.stringify(specBuilder_1.spec, null, 2);
const swaggerUiHtml = `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>PartyManagement API — Swagger UI</title>
  <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5/swagger-ui.css" />
</head>
<body>
  <div id="swagger-ui"></div>
  <script src="https://unpkg.com/swagger-ui-dist@5/swagger-ui-bundle.js"></script>
  <script>
    SwaggerUIBundle({
      url: '/api/openapi.json',
      dom_id: '#swagger-ui',
      presets: [SwaggerUIBundle.presets.apis, SwaggerUIBundle.SwaggerUIStandalonePreset],
      layout: 'BaseLayout',
    });
  </script>
</body>
</html>`;
async function getOpenApiSpecHandler(_req, _ctx) {
    return {
        status: 200,
        body: specJson,
        headers: { 'Content-Type': 'application/json' },
    };
}
async function getSwaggerUiHandler(_req, _ctx) {
    return {
        status: 200,
        body: swaggerUiHtml,
        headers: { 'Content-Type': 'text/html' },
    };
}
// ── Function registrations ──────────────────────────────────────────────────
functions_1.app.http('getOpenApiSpec', {
    methods: ['GET'],
    route: 'openapi.json',
    authLevel: 'anonymous',
    handler: getOpenApiSpecHandler,
});
functions_1.app.http('getSwaggerUi', {
    methods: ['GET'],
    route: 'swagger/ui',
    authLevel: 'anonymous',
    handler: getSwaggerUiHandler,
});
//# sourceMappingURL=swaggerFunction.js.map