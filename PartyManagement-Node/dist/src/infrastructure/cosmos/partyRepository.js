"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.PartyRepository = void 0;
const party_1 = require("../../domain/entities/party");
function toDocument(party) {
    return {
        id: party.id,
        officeId: party.officeId,
        firstName: party.firstName,
        lastName: party.lastName,
        address: party.address,
        description: party.description,
        phone: party.phone,
        identificationNumber: party.identificationNumber,
    };
}
function fromDocument(doc) {
    return new party_1.Party(doc.id, doc.officeId, doc.firstName, doc.lastName, doc.address, doc.description, doc.phone, doc.identificationNumber);
}
function isNotFound(err) {
    return (typeof err === 'object' &&
        err !== null &&
        'code' in err &&
        err.code === 404);
}
class PartyRepository {
    container;
    constructor(cosmosService, containerId) {
        // Container reference is resolved once; actual network call happens on first operation.
        this.container = cosmosService.getContainer(containerId);
    }
    async add(party) {
        return this.upsert(party);
    }
    async update(party) {
        return this.upsert(party);
    }
    async upsert(party) {
        const { resource } = await this.container.items.upsert(toDocument(party));
        if (!resource)
            throw new Error('Upsert did not return a resource.');
        return fromDocument(resource);
    }
    async get(partyId, officeId) {
        try {
            const { resource } = await this.container
                .item(partyId, officeId)
                .read();
            return resource ? fromDocument(resource) : null;
        }
        catch (err) {
            if (isNotFound(err))
                return null;
            throw err;
        }
    }
    async getAll(officeId) {
        const querySpec = {
            query: 'SELECT * FROM c WHERE c.officeId = @officeId',
            parameters: [{ name: '@officeId', value: officeId }],
        };
        const iterator = this.container.items.query(querySpec, {
            partitionKey: officeId,
        });
        const { resources } = await iterator.fetchAll();
        return resources.map(fromDocument);
    }
    async getCount(officeId) {
        const querySpec = {
            query: 'SELECT VALUE COUNT(1) FROM c WHERE c.officeId = @officeId',
            parameters: [{ name: '@officeId', value: officeId }],
        };
        const iterator = this.container.items.query(querySpec);
        const { resources } = await iterator.fetchAll();
        return resources[0] ?? 0;
    }
    async existByIdentificationNumber(officeId, identificationNumber, currentPartyId) {
        let query = 'SELECT VALUE COUNT(1) FROM c WHERE c.officeId = @officeId AND c.identificationNumber = @identificationNumber';
        const parameters = [
            { name: '@officeId', value: officeId },
            { name: '@identificationNumber', value: identificationNumber },
        ];
        if (currentPartyId?.trim()) {
            query += ' AND c.id != @currentPartyId';
            parameters.push({ name: '@currentPartyId', value: currentPartyId });
        }
        const iterator = this.container.items.query({ query, parameters });
        const { resources } = await iterator.fetchAll();
        return (resources[0] ?? 0) > 0;
    }
    async delete(partyId, officeId) {
        try {
            await this.container.item(partyId, officeId).delete();
        }
        catch (err) {
            if (isNotFound(err))
                return; // Idempotent — swallow 404
            throw err;
        }
    }
}
exports.PartyRepository = PartyRepository;
//# sourceMappingURL=partyRepository.js.map