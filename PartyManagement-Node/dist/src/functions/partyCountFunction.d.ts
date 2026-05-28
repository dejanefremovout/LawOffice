import { HttpRequest, HttpResponseInit, InvocationContext } from '@azure/functions';
import { IClientService } from '../application/services/iClientService';
import { IOpposingPartyService } from '../application/services/iOpposingPartyService';
export declare function getPartyCountHandler(req: HttpRequest, context: InvocationContext, clientService: IClientService, opposingPartyService: IOpposingPartyService): Promise<HttpResponseInit>;
//# sourceMappingURL=partyCountFunction.d.ts.map