import { HttpRequest, HttpResponseInit, InvocationContext } from '@azure/functions';
import { IOpposingPartyDeletedPublisher } from '../application/messaging/iOpposingPartyDeletedPublisher';
import { IOpposingPartyService } from '../application/services/iOpposingPartyService';
export declare function getOpposingPartyHandler(req: HttpRequest, context: InvocationContext, service: IOpposingPartyService): Promise<HttpResponseInit>;
export declare function getAllOpposingPartiesHandler(req: HttpRequest, context: InvocationContext, service: IOpposingPartyService): Promise<HttpResponseInit>;
export declare function createOpposingPartyHandler(req: HttpRequest, context: InvocationContext, service: IOpposingPartyService): Promise<HttpResponseInit>;
export declare function updateOpposingPartyHandler(req: HttpRequest, context: InvocationContext, service: IOpposingPartyService): Promise<HttpResponseInit>;
export declare function deleteOpposingPartyHandler(req: HttpRequest, context: InvocationContext, service: IOpposingPartyService, publisher: IOpposingPartyDeletedPublisher): Promise<HttpResponseInit>;
//# sourceMappingURL=opposingPartyFunctions.d.ts.map