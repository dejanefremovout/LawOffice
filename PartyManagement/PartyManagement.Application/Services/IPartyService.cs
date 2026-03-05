using PartyManagement.Domain.ViewModels;

namespace PartyManagement.Application.Services;

/// <summary>
/// Base application service contract for party operations.
/// </summary>
public interface IPartyService
{
    /// <summary>
    /// Gets all parties for an office.
    /// </summary>
    Task<IEnumerable<PartyModel>> GetAll(string officeId);

    /// <summary>
    /// Gets a party by identifier.
    /// </summary>
    Task<PartyModel?> Get(string partyId, string officeId);

    /// <summary>
    /// Creates a new party.
    /// </summary>
    Task<PartyModel> Create(PartyCreateModel partyModel);

    /// <summary>
    /// Updates an existing party.
    /// </summary>
    Task<PartyModel> Update(PartyModel partyModel);

    /// <summary>
    /// Gets party count for an office.
    /// </summary>
    Task<int> GetCount(string officeId);
}
