import { ServiceBusSender } from '@azure/service-bus';
import { IOpposingPartyDeletedPublisher } from '../../application/messaging/iOpposingPartyDeletedPublisher';
import { OpposingPartyDeletedMessage } from '../../application/messaging/opposingPartyDeletedMessage';
export declare class ServiceBusOpposingPartyDeletedPublisher implements IOpposingPartyDeletedPublisher {
    private readonly sender;
    constructor(sender: ServiceBusSender);
    publish(message: OpposingPartyDeletedMessage, cancellationToken?: AbortSignal): Promise<void>;
}
//# sourceMappingURL=serviceBusOpposingPartyDeletedPublisher.d.ts.map