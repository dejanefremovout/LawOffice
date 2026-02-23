using Microsoft.Azure.Cosmos;

namespace PartyManagement.Infrastructure.Data;

public class CosmosService : ICosmosService
{
    private readonly CosmosClient _cosmosClient;
    //private const string _cosmosDatabaseId = "partymanagement";
    private const string _cosmosDatabaseId = "officemanagement";

    public CosmosService(CosmosClient cosmosClient)
    {
        ArgumentNullException.ThrowIfNull(cosmosClient);

        _cosmosClient = cosmosClient;
    }

    public Container GetContainer(string containerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(containerId);
        return _cosmosClient.GetContainer(_cosmosDatabaseId, containerId);
    }
}
