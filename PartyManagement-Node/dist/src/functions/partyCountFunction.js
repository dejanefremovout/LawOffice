"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getPartyCountHandler = getPartyCountHandler;
const functions_1 = require("@azure/functions");
const container_1 = require("../container");
const officeIdHelper_1 = require("./helpers/officeIdHelper");
async function getPartyCountHandler(req, context, clientService, opposingPartyService) {
    try {
        const officeId = (0, officeIdHelper_1.getOfficeId)(req);
        const [clientsCount, opposingPartiesCount] = await Promise.all([
            clientService.getCount(officeId),
            opposingPartyService.getCount(officeId),
        ]);
        const result = { clientsCount, opposingPartiesCount };
        return { status: 200, jsonBody: result };
    }
    catch (err) {
        if (err instanceof Error) {
            context.warn(err.message);
            return { status: 400, body: err.message };
        }
        context.error('Unexpected error in partyCountFunction:', err);
        return { status: 500, body: 'An unexpected error occurred.' };
    }
}
// ── Function registration ───────────────────────────────────────────────────
functions_1.app.http('getPartyCount', {
    methods: ['GET'],
    route: 'party/count',
    authLevel: 'function',
    handler: (req, ctx) => getPartyCountHandler(req, ctx, container_1.container.createClientService(), container_1.container.createOpposingPartyService()),
});
//# sourceMappingURL=partyCountFunction.js.map