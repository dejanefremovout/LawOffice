import { Party } from '../../domain/entities/party';
import { IPartyRepository } from '../../domain/interfaces/iPartyRepository';
import { ICosmosService } from './cosmosService';
export declare abstract class PartyRepository implements IPartyRepository {
    private readonly container;
    constructor(cosmosService: ICosmosService, containerId: string);
    add(party: Party): Promise<Party>;
    update(party: Party): Promise<Party>;
    private upsert;
    get(partyId: string, officeId: string): Promise<Party | null>;
    getAll(officeId: string): Promise<Party[]>;
    getCount(officeId: string): Promise<number>;
    existByIdentificationNumber(officeId: string, identificationNumber: string, currentPartyId?: string): Promise<boolean>;
    delete(partyId: string, officeId: string): Promise<void>;
}
//# sourceMappingURL=partyRepository.d.ts.map