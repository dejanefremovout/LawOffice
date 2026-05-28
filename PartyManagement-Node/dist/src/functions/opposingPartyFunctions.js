"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getOpposingPartyHandler = getOpposingPartyHandler;
exports.getAllOpposingPartiesHandler = getAllOpposingPartiesHandler;
exports.createOpposingPartyHandler = createOpposingPartyHandler;
exports.updateOpposingPartyHandler = updateOpposingPartyHandler;
exports.deleteOpposingPartyHandler = deleteOpposingPartyHandler;
const functions_1 = require("@azure/functions");
const container_1 = require("../container");
const officeIdHelper_1 = require("./helpers/officeIdHelper");
function handleError(err, context) {
    if (err instanceof Error) {
        context.warn(err.message);
        return { status: 400, body: err.message };
    }
    context.error('Unexpected error in opposingPartyFunctions:', err);
    return { status: 500, body: 'An unexpected error occurred.' };
}
async function getOpposingPartyHandler(req, context, service) {
    try {
        const officeId = (0, officeIdHelper_1.getOfficeId)(req);
        const opposingPartyId = req.params['opposingPartyId'];
        if (!opposingPartyId) {
            return { status: 400, body: 'opposingPartyId route parameter is required.' };
        }
        const result = await service.get(opposingPartyId, officeId);
        if (!result) {
            return { status: 404, body: 'Opposing party not found.' };
        }
        return { status: 200, jsonBody: result };
    }
    catch (err) {
        return handleError(err, context);
    }
}
async function getAllOpposingPartiesHandler(req, context, service) {
    try {
        const officeId = (0, officeIdHelper_1.getOfficeId)(req);
        const result = await service.getAll(officeId);
        return { status: 200, jsonBody: result };
    }
    catch (err) {
        return handleError(err, context);
    }
}
async function createOpposingPartyHandler(req, context, service) {
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
            headers: { Location: `/opposingParty/${result.officeId}/${result.id}` },
        };
    }
    catch (err) {
        return handleError(err, context);
    }
}
async function updateOpposingPartyHandler(req, context, service) {
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
async function deleteOpposingPartyHandler(req, context, service, publisher) {
    try {
        const officeId = (0, officeIdHelper_1.getOfficeId)(req);
        const opposingPartyId = req.params['opposingPartyId'];
        if (!opposingPartyId) {
            return { status: 400, body: 'opposingPartyId route parameter is required.' };
        }
        await service.delete(opposingPartyId, officeId);
        const message = {
            officeId,
            opposingPartyId,
            occurredAtUtc: new Date(),
        };
        await publisher.publish(message);
        return { status: 204 };
    }
    catch (err) {
        return handleError(err, context);
    }
}
// ── Function registrations ──────────────────────────────────────────────────
functions_1.app.http('getOpposingParty', {
    methods: ['GET'],
    route: 'opposingParty/{opposingPartyId}',
    authLevel: 'function',
    handler: (req, ctx) => getOpposingPartyHandler(req, ctx, container_1.container.createOpposingPartyService()),
});
functions_1.app.http('getAllOpposingParties', {
    methods: ['GET'],
    route: 'opposingParty',
    authLevel: 'function',
    handler: (req, ctx) => getAllOpposingPartiesHandler(req, ctx, container_1.container.createOpposingPartyService()),
});
functions_1.app.http('createOpposingParty', {
    methods: ['POST'],
    route: 'opposingParty',
    authLevel: 'function',
    handler: (req, ctx) => createOpposingPartyHandler(req, ctx, container_1.container.createOpposingPartyService()),
});
functions_1.app.http('updateOpposingParty', {
    methods: ['PUT'],
    route: 'opposingParty',
    authLevel: 'function',
    handler: (req, ctx) => updateOpposingPartyHandler(req, ctx, container_1.container.createOpposingPartyService()),
});
functions_1.app.http('deleteOpposingParty', {
    methods: ['DELETE'],
    route: 'opposingParty/{opposingPartyId}',
    authLevel: 'function',
    handler: (req, ctx) => deleteOpposingPartyHandler(req, ctx, container_1.container.createOpposingPartyService(), container_1.container.createOpposingPartyDeletedPublisher()),
});
//# sourceMappingURL=opposingPartyFunctions.js.map