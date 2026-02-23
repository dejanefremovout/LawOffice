using Microsoft.Azure.Cosmos;

namespace OfficeManagement.Infrastructure.Data;

public interface ICosmosService
{
    Container GetContainer(string containerId);
}
