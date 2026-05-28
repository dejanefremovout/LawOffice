"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getClientHandler = getClientHandler;
exports.getAllClientsHandler = getAllClientsHandler;
exports.createClientHandler = createClientHandler;
exports.updateClientHandler = updateClientHandler;
const functions_1 = require("@azure/functions");
const container_1 = require("../container");
const officeIdHelper_1 = require("./helpers/officeIdHelper");
function handleError(err, context) {
    if (err instanceof Error) {
        context.warn(err.message);
        return { status: 400, body: err.message };
    }
    context.error('Unexpected error in clientFunctions:', err);
    return { status: 500, body: 'An unexpected error occurred.' };
}
async function getClientHandler(req, context, service) {
    try {
        const officeId = (0, officeIdHelper_1.getOfficeId)(req);
        const clientId = req.params['clientId'];
        if (!clientId) {
            return { status: 400, body: 'clientId route parameter is required.' };
        }
        const result = await service.get(clientId, officeId);
        if (!result) {
            return { status: 404, body: 'Client not found.' };
        }
        return { status: 200, jsonBody: result };
    }
    catch (err) {
        return handleError(err, context);
    }
}
async function getAllClientsHandler(req, context, service) {
    try {
        const officeId = (0, officeIdHelper_1.getOfficeId)(req);
        const result = await service.getAll(officeId);
        return { status: 200, jsonBody: result };
    }
    catch (err) {
        return handleError(err, context);
    }
}
async function createClientHandler(req, context, service) {
    try {
        const officeId = (0, officeIdHelper_1.getOfficeId)(req);
        let body;
        try {
            body = (await req.json());
        }
        catch {
            return { status: 400, body: 'Invalid request body.' };
        }
        if (!body) {
            return { status: 400, body: 'Request body is required.' };
        }
        const createModel = {
            ...body,
            officeId, // Always sourced from the X-Office-Id header, never from body
        };
        const result = await service.create(createModel);
        return {
            status: 201,
            jsonBody: result,
            headers: { Location: `/client/${result.officeId}/${result.id}` },
        };
    }
    catch (err) {
        return handleError(err, context);
    }
}
async function updateClientHandler(req, context, service) {
    try {
        const officeId = (0, officeIdHelper_1.getOfficeId)(req);
        let body;
        try {
            body = (await req.json());
        }
        catch {
            return { status: 400, body: 'Invalid request body.' };
        }
        if (!body) {
            return { status: 400, body: 'Request body is required.' };
        }
        const updateModel = {
            ...body,
            officeId, // Always sourced from the X-Office-Id header, never from body
        };
        const result = await service.update(updateModel);
        return { status: 200, jsonBody: result };
    }
    catch (err) {
        return handleError(err, context);
    }
}
// ── Function registrations ──────────────────────────────────────────────────
functions_1.app.http('getClient', {
    methods: ['GET'],
    route: 'client/{clientId}',
    authLevel: 'function',
    handler: (req, ctx) => getClientHandler(req, ctx, container_1.container.createClientService()),
});
functions_1.app.http('getAllClients', {
    methods: ['GET'],
    route: 'client',
    authLevel: 'function',
    handler: (req, ctx) => getAllClientsHandler(req, ctx, container_1.container.createClientService()),
});
functions_1.app.http('createClient', {
    methods: ['POST'],
    route: 'client',
    authLevel: 'function',
    handler: (req, ctx) => createClientHandler(req, ctx, container_1.container.createClientService()),
});
functions_1.app.http('updateClient', {
    methods: ['PUT'],
    route: 'client',
    authLevel: 'function',
    handler: (req, ctx) => updateClientHandler(req, ctx, container_1.container.createClientService()),
});
//# sourceMappingURL=clientFunctions.js.map