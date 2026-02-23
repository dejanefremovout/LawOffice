using PartyManagement.Domain.Interfaces;
using PartyManagement.Infrastructure.Data;

namespace PartyManagement.Infrastructure.Repositories;

public class OpposingPartyRepository(ICosmosService cosmosService) : PartyRepository(cosmosService), IOpposingPartyRepository
{
    protected override string PartiesContainerId => "opposingparties";
}
