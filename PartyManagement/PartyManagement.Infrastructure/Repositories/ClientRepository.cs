using PartyManagement.Domain.Interfaces;
using PartyManagement.Infrastructure.Data;

namespace PartyManagement.Infrastructure.Repositories;

public class ClientRepository(ICosmosService cosmosService) : PartyRepository(cosmosService), IClientRepository
{
    protected override string PartiesContainerId => "clients";
}
