"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.spec = void 0;
const zod_to_openapi_1 = require("@asteasolutions/zod-to-openapi");
const zod_1 = require("zod");
const schemas_1 = require("./schemas");
// Reusable $ref to the registered PartyModel component schema.
const partyModelRef = { $ref: '#/components/schemas/PartyModel' };
// The header parameter is the Zod schema itself — the library reads its openapi
// metadata (param.name / param.in) to generate the parameter object.
const officeIdHeaders = [schemas_1.OfficeIdHeaderParam];
function buildSpec() {
    // ── Client routes ─────────────────────────────────────────────────────────
    schemas_1.registry.registerPath({
        method: 'get',
        path: '/client',
        tags: ['Client'],
        summary: 'Get all clients for an office.',
        request: { headers: officeIdHeaders },
        responses: {
            200: {
                description: 'List of clients.',
                content: { 'application/json': { schema: { type: 'array', items: partyModelRef } } },
            },
            400: { description: 'Missing or invalid X-Office-Id header.' },
        },
    });
    schemas_1.registry.registerPath({
        method: 'get',
        path: '/client/{clientId}',
        tags: ['Client'],
        summary: 'Get a client by ID.',
        request: {
            headers: officeIdHeaders,
            params: zod_1.z.object({ clientId: zod_1.z.string() }),
        },
        responses: {
            200: { description: 'The client.', content: { 'application/json': { schema: partyModelRef } } },
            400: { description: 'Invalid request.' },
            404: { description: 'Client not found.' },
        },
    });
    schemas_1.registry.registerPath({
        method: 'post',
        path: '/client',
        tags: ['Client'],
        summary: 'Create a new client.',
        request: {
            headers: officeIdHeaders,
            body: { content: { 'application/json': { schema: schemas_1.PartyCreateModelSchema } }, required: true },
        },
        responses: {
            201: { description: 'Client created.', content: { 'application/json': { schema: partyModelRef } } },
            400: { description: 'Validation error.' },
        },
    });
    schemas_1.registry.registerPath({
        method: 'put',
        path: '/client',
        tags: ['Client'],
        summary: 'Update an existing client.',
        request: {
            headers: officeIdHeaders,
            body: { content: { 'application/json': { schema: schemas_1.PartyModelSchema } }, required: true },
        },
        responses: {
            200: { description: 'Updated client.', content: { 'application/json': { schema: partyModelRef } } },
            400: { description: 'Validation error.' },
        },
    });
    // ── Opposing party routes ─────────────────────────────────────────────────
    schemas_1.registry.registerPath({
        method: 'get',
        path: '/opposingParty',
        tags: ['OpposingParty'],
        summary: 'Get all opposing parties for an office.',
        request: { headers: officeIdHeaders },
        responses: {
            200: {
                description: 'List of opposing parties.',
                content: { 'application/json': { schema: { type: 'array', items: partyModelRef } } },
            },
            400: { description: 'Missing or invalid X-Office-Id header.' },
        },
    });
    schemas_1.registry.registerPath({
        method: 'get',
        path: '/opposingParty/{opposingPartyId}',
        tags: ['OpposingParty'],
        summary: 'Get an opposing party by ID.',
        request: {
            headers: officeIdHeaders,
            params: zod_1.z.object({ opposingPartyId: zod_1.z.string() }),
        },
        responses: {
            200: { description: 'The opposing party.', content: { 'application/json': { schema: partyModelRef } } },
            400: { description: 'Invalid request.' },
            404: { description: 'Opposing party not found.' },
        },
    });
    schemas_1.registry.registerPath({
        method: 'post',
        path: '/opposingParty',
        tags: ['OpposingParty'],
        summary: 'Create a new opposing party.',
        request: {
            headers: officeIdHeaders,
            body: { content: { 'application/json': { schema: schemas_1.PartyCreateModelSchema } }, required: true },
        },
        responses: {
            201: { description: 'Opposing party created.', content: { 'application/json': { schema: partyModelRef } } },
            400: { description: 'Validation error.' },
        },
    });
    schemas_1.registry.registerPath({
        method: 'put',
        path: '/opposingParty',
        tags: ['OpposingParty'],
        summary: 'Update an existing opposing party.',
        request: {
            headers: officeIdHeaders,
            body: { content: { 'application/json': { schema: schemas_1.PartyModelSchema } }, required: true },
        },
        responses: {
            200: { description: 'Updated opposing party.', content: { 'application/json': { schema: partyModelRef } } },
            400: { description: 'Validation error.' },
        },
    });
    schemas_1.registry.registerPath({
        method: 'delete',
        path: '/opposingParty/{opposingPartyId}',
        tags: ['OpposingParty'],
        summary: 'Delete an opposing party and publish a deletion event.',
        request: {
            headers: officeIdHeaders,
            params: zod_1.z.object({ opposingPartyId: zod_1.z.string() }),
        },
        responses: {
            204: { description: 'Opposing party deleted.' },
            400: { description: 'Invalid request.' },
        },
    });
    // ── Party count route ─────────────────────────────────────────────────────
    schemas_1.registry.registerPath({
        method: 'get',
        path: '/party/count',
        tags: ['Party'],
        summary: 'Get aggregate party counts for an office.',
        request: { headers: officeIdHeaders },
        responses: {
            200: {
                description: 'Party counts.',
                content: { 'application/json': { schema: schemas_1.PartyCountModelSchema } },
            },
            400: { description: 'Missing or invalid X-Office-Id header.' },
        },
    });
    return new zod_to_openapi_1.OpenApiGeneratorV3(schemas_1.registry.definitions).generateDocument({
        openapi: '3.0.0',
        info: {
            title: 'PartyManagement API',
            version: '1.0.0',
            description: 'Manages clients and opposing parties for a law office.',
        },
        servers: [{ url: '/api' }],
    });
}
// Built once at module load and cached for the lifetime of the process.
exports.spec = buildSpec();
//# sourceMappingURL=specBuilder.js.map