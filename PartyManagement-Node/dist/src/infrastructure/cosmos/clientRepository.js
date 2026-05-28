"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ClientRepository = void 0;
const partyRepository_1 = require("./partyRepository");
class ClientRepository extends partyRepository_1.PartyRepository {
    constructor(cosmosService) {
        super(cosmosService, 'clients');
    }
}
exports.ClientRepository = ClientRepository;
//# sourceMappingURL=clientRepository.js.map