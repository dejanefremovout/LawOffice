"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.PartyService = void 0;
const party_1 = require("../../domain/entities/party");
const partyModel_1 = require("../../domain/models/partyModel");
class PartyService {
    repository;
    constructor(repository) {
        this.repository = repository;
    }
    async getAll(officeId) {
        const parties = await this.repository.getAll(officeId);
        return parties.map(partyModel_1.toPartyModel);
    }
    async get(partyId, officeId) {
        const party = await this.repository.get(partyId, officeId);
        return party ? (0, partyModel_1.toPartyModel)(party) : null;
    }
    async create(model) {
        if (model.identificationNumber?.trim() &&
            (await this.repository.existByIdentificationNumber(model.officeId, model.identificationNumber))) {
            throw new Error('A party with the same identification number already exists in the office.');
        }
        const party = party_1.Party.create(model.officeId, model.firstName, model.lastName, model.address, model.description, model.phone, model.identificationNumber);
        const saved = await this.repository.add(party);
        return (0, partyModel_1.toPartyModel)(saved);
    }
    async update(model) {
        if (model.identificationNumber?.trim() &&
            (await this.repository.existByIdentificationNumber(model.officeId, model.identificationNumber, model.id))) {
            throw new Error('A party with the same identification number already exists in the office.');
        }
        const party = await this.repository.get(model.id, model.officeId);
        if (!party) {
            throw new Error('Party not found.');
        }
        party.setName(model.firstName, model.lastName);
        party.setAddress(model.address);
        party.setDescription(model.description);
        party.setPhone(model.phone);
        party.setIdentificationNumber(model.identificationNumber);
        const saved = await this.repository.update(party);
        return (0, partyModel_1.toPartyModel)(saved);
    }
    async getCount(officeId) {
        return this.repository.getCount(officeId);
    }
    async delete(partyId, officeId) {
        const party = await this.repository.get(partyId, officeId);
        if (!party) {
            throw new Error('Party not found.');
        }
        await this.repository.delete(partyId, officeId);
    }
}
exports.PartyService = PartyService;
//# sourceMappingURL=partyService.js.map