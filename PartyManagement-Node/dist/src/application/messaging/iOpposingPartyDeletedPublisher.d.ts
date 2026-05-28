import { OpposingPartyDeletedMessage } from './opposingPartyDeletedMessage';
export interface IOpposingPartyDeletedPublisher {
    publish(message: OpposingPartyDeletedMessage, cancellationToken?: AbortSignal): Promise<void>;
}
//# sourceMappingURL=iOpposingPartyDeletedPublisher.d.ts.map