import { Party } from '../entities/party';
export type PartyModel = {
    readonly id: string;
    readonly officeId: string;
    readonly firstName: string;
    readonly lastName: string;
    readonly address?: string;
    readonly description?: string;
    readonly phone?: string;
    readonly identificationNumber?: string;
};
export declare function toPartyModel(party: Party): PartyModel;
//# sourceMappingURL=partyModel.d.ts.map