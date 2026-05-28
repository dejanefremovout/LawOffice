import { IClientRepository } from '../../domain/interfaces/iClientRepository';
import { ICosmosService } from './cosmosService';
import { PartyRepository } from './partyRepository';
export declare class ClientRepository extends PartyRepository implements IClientRepository {
    constructor(cosmosService: ICosmosService);
}
//# sourceMappingURL=clientRepository.d.ts.map