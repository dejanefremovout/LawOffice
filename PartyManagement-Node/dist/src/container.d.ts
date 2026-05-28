import { IOpposingPartyDeletedPublisher } from './application/messaging/iOpposingPartyDeletedPublisher';
import { IClientService } from './application/services/iClientService';
import { IOpposingPartyService } from './application/services/iOpposingPartyService';
export interface IContainer {
    createClientService(): IClientService;
    createOpposingPartyService(): IOpposingPartyService;
    createOpposingPartyDeletedPublisher(): IOpposingPartyDeletedPublisher;
}
export declare const container: IContainer;
//# sourceMappingURL=container.d.ts.map