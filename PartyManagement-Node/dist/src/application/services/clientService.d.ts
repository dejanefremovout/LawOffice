import { IClientRepository } from '../../domain/interfaces/iClientRepository';
import { PartyService } from './partyService';
import { IClientService } from './iClientService';
export declare class ClientService extends PartyService implements IClientService {
    constructor(repository: IClientRepository);
}
//# sourceMappingURL=clientService.d.ts.map