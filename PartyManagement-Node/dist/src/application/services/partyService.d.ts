import { IPartyRepository } from '../../domain/interfaces/iPartyRepository';
import { PartyCreateModel } from '../../domain/models/partyCreateModel';
import { PartyModel } from '../../domain/models/partyModel';
import { IPartyService } from './iPartyService';
export declare abstract class PartyService implements IPartyService {
    private readonly repository;
    constructor(repository: IPartyRepository);
    getAll(officeId: string): Promise<PartyModel[]>;
    get(partyId: string, officeId: string): Promise<PartyModel | null>;
    create(model: PartyCreateModel): Promise<PartyModel>;
    update(model: PartyModel): Promise<PartyModel>;
    getCount(officeId: string): Promise<number>;
    delete(partyId: string, officeId: string): Promise<void>;
}
//# sourceMappingURL=partyService.d.ts.map