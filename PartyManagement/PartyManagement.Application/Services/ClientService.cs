using PartyManagement.Domain.Interfaces;

namespace PartyManagement.Application.Services;

public class ClientService(IClientRepository partyRepository) : PartyService(partyRepository), IClientService
{
}
