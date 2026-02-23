using PartyManagement.Domain.Interfaces;

namespace PartyManagement.Application.Services;

public class OpposingPartyService(IOpposingPartyRepository partyRepository) : PartyService(partyRepository), IOpposingPartyService
{
}
