"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ClientService = void 0;
const partyService_1 = require("./partyService");
class ClientService extends partyService_1.PartyService {
    constructor(repository) {
        super(repository);
    }
}
exports.ClientService = ClientService;
//# sourceMappingURL=clientService.js.map