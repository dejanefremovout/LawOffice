import { Party } from '../entities/party';
export interface IPartyRepository {
    add(party: Party): Promise<Party>;
    update(party: Party): Promise<Party>;
    get(partyId: string, officeId: string): Promise<Party | null>;
    getAll(officeId: string): Promise<Party[]>;
    getCount(officeId: string): Promise<number>;
    delete(partyId: string, officeId: string): Promise<void>;
    existByIdentificationNumber(officeId: string, identificationNumber: string, currentPartyId?: string): Promise<boolean>;
}
//# sourceMappingURL=iPartyRepository.d.ts.map