using Microsoft.Azure.Cosmos;

namespace CaseManagement.Infrastructure.Data;

public interface ICosmosService
{
    Container GetContainer(string containerId);
}
