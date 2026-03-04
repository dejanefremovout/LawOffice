using PartyManagement.Domain.ViewModels;

namespace PartyManagement.Application.Services;

public interface IPartyService
{
    Task<IEnumerable<PartyModel>> GetAll(string officeId);

    Task<PartyModel?> Get(string partyId, string officeId);

    Task<PartyModel> Create(PartyCreateModel partyModel);

    Task<PartyModel> Update(PartyModel partyModel);

    Task<int> GetCount(string officeId);
}
