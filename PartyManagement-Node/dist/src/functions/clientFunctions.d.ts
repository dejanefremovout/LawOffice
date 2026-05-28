import { HttpRequest, HttpResponseInit, InvocationContext } from '@azure/functions';
import { IClientService } from '../application/services/iClientService';
export declare function getClientHandler(req: HttpRequest, context: InvocationContext, service: IClientService): Promise<HttpResponseInit>;
export declare function getAllClientsHandler(req: HttpRequest, context: InvocationContext, service: IClientService): Promise<HttpResponseInit>;
export declare function createClientHandler(req: HttpRequest, context: InvocationContext, service: IClientService): Promise<HttpResponseInit>;
export declare function updateClientHandler(req: HttpRequest, context: InvocationContext, service: IClientService): Promise<HttpResponseInit>;
//# sourceMappingURL=clientFunctions.d.ts.map