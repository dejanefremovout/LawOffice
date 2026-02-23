using Microsoft.Azure.Cosmos;

namespace PartyManagement.Infrastructure.Data;

public interface ICosmosService
{
    Container GetContainer(string containerId);
}
