using LawOffice.Domain.Interfaces;
using PartyManagement.Domain.Entities;

namespace PartyManagement.Domain.Interfaces;

public interface IPartyRepository : IRepository<Party>
{
    Task<Party> Add(Party party);

    Task<Party> Update(Party party);

    Task<Party?> Get(string partyId, string officeId);

    Task<bool> ExistByIdentificationNumber(string officeId, string identificationNumber, string? currentPartyId = null);

    Task<IEnumerable<Party>> GetAll(string officeId);
}