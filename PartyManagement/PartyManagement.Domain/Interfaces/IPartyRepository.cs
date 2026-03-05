using LawOffice.Domain.Interfaces;
using PartyManagement.Domain.Entities;

namespace PartyManagement.Domain.Interfaces;

/// <summary>
/// Repository contract for party aggregate persistence.
/// </summary>
public interface IPartyRepository : IRepository<Party>
{
    /// <summary>
    /// Persists a new party.
    /// </summary>
    Task<Party> Add(Party party);

    /// <summary>
    /// Persists updates to an existing party.
    /// </summary>
    Task<Party> Update(Party party);

    /// <summary>
    /// Gets a party by identifier and office scope.
    /// </summary>
    Task<Party?> Get(string partyId, string officeId);

    /// <summary>
    /// Checks if a party with the same identification number already exists.
    /// </summary>
    Task<bool> ExistByIdentificationNumber(string officeId, string identificationNumber, string? currentPartyId = null);

    /// <summary>
    /// Gets all parties for an office.
    /// </summary>
    Task<IEnumerable<Party>> GetAll(string officeId);

    /// <summary>
    /// Gets party count for an office.
    /// </summary>
    Task<int> GetCount(string officeId);
}