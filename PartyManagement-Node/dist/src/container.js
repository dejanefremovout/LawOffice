"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.container = void 0;
const cosmos_1 = require("@azure/cosmos");
const service_bus_1 = require("@azure/service-bus");
const clientService_1 = require("./application/services/clientService");
const opposingPartyService_1 = require("./application/services/opposingPartyService");
const cosmosService_1 = require("./infrastructure/cosmos/cosmosService");
const clientRepository_1 = require("./infrastructure/cosmos/clientRepository");
const opposingPartyRepository_1 = require("./infrastructure/cosmos/opposingPartyRepository");
const serviceBusOpposingPartyDeletedPublisher_1 = require("./infrastructure/serviceBus/serviceBusOpposingPartyDeletedPublisher");
// ── Fail-fast env var validation ────────────────────────────────────────────
function requireEnv(name) {
    const value = process.env[name];
    if (!value?.trim()) {
        throw new Error(`Required environment variable "${name}" is missing or empty.`);
    }
    return value.trim();
}
const cosmosConnectionString = requireEnv('COSMOS_CONNECTION_STRING');
const serviceBusConnectionString = requireEnv('SERVICE_BUS_CONNECTION_STRING');
const opposingPartyDeletedQueueName = requireEnv('OPPOSING_PARTY_DELETED_QUEUE_NAME');
// ── Singletons — created once per process ───────────────────────────────────
const cosmosClient = new cosmos_1.CosmosClient(cosmosConnectionString);
const serviceBusClient = new service_bus_1.ServiceBusClient(serviceBusConnectionString);
// ServiceBusSender is lightweight and safe to create once per queue name.
const serviceBusSender = serviceBusClient.createSender(opposingPartyDeletedQueueName);
// ── Container — factory functions create per-invocation service instances ───
exports.container = {
    createClientService() {
        const cosmosService = new cosmosService_1.CosmosService(cosmosClient);
        const repository = new clientRepository_1.ClientRepository(cosmosService);
        return new clientService_1.ClientService(repository);
    },
    createOpposingPartyService() {
        const cosmosService = new cosmosService_1.CosmosService(cosmosClient);
        const repository = new opposingPartyRepository_1.OpposingPartyRepository(cosmosService);
        return new opposingPartyService_1.OpposingPartyService(repository);
    },
    createOpposingPartyDeletedPublisher() {
        return new serviceBusOpposingPartyDeletedPublisher_1.ServiceBusOpposingPartyDeletedPublisher(serviceBusSender);
    },
};
//# sourceMappingURL=container.js.map