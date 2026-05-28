"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.toPartyModel = toPartyModel;
function toPartyModel(party) {
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
//# sourceMappingURL=partyModel.js.map