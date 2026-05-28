import { IOpposingPartyRepository } from '../../domain/interfaces/iOpposingPartyRepository';
import { ICosmosService } from './cosmosService';
import { PartyRepository } from './partyRepository';
export declare class OpposingPartyRepository extends PartyRepository implements IOpposingPartyRepository {
    constructor(cosmosService: ICosmosService);
}
//# sourceMappingURL=opposingPartyRepository.d.ts.map