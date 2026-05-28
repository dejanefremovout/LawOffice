"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.OpposingPartyRepository = void 0;
const partyRepository_1 = require("./partyRepository");
class OpposingPartyRepository extends partyRepository_1.PartyRepository {
    constructor(cosmosService) {
        super(cosmosService, 'opposingparties');
    }
}
exports.OpposingPartyRepository = OpposingPartyRepository;
//# sourceMappingURL=opposingPartyRepository.js.map