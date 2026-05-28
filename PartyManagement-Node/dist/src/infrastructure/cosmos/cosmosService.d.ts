import { Container, CosmosClient } from '@azure/cosmos';
export interface ICosmosService {
    getContainer(containerId: string): Container;
}
export declare class CosmosService implements ICosmosService {
    private readonly client;
    constructor(client: CosmosClient);
    getContainer(containerId: string): Container;
}
//# sourceMappingURL=cosmosService.d.ts.map