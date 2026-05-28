"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.CosmosService = void 0;
const COSMOS_DATABASE_ID = 'partymanagement';
class CosmosService {
    client;
    constructor(client) {
        this.client = client;
        if (!client)
            throw new Error('CosmosClient is required.');
    }
    getContainer(containerId) {
        if (!containerId?.trim())
            throw new Error('containerId is required.');
        return this.client.database(COSMOS_DATABASE_ID).container(containerId);
    }
}
exports.CosmosService = CosmosService;
//# sourceMappingURL=cosmosService.js.map