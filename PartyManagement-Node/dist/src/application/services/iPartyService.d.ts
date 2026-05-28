import { PartyCreateModel } from '../../domain/models/partyCreateModel';
import { PartyModel } from '../../domain/models/partyModel';
export interface IPartyService {
    getAll(officeId: string): Promise<PartyModel[]>;
    get(partyId: string, officeId: string): Promise<PartyModel | null>;
    create(model: PartyCreateModel): Promise<PartyModel>;
    update(model: PartyModel): Promise<PartyModel>;
    getCount(officeId: string): Promise<number>;
    delete(partyId: string, officeId: string): Promise<void>;
}
//# sourceMappingURL=iPartyService.d.ts.map