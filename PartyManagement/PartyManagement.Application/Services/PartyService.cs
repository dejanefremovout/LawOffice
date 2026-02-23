using PartyManagement.Domain.Entities;
using PartyManagement.Domain.Interfaces;
using PartyManagement.Domain.ViewModels;

namespace PartyManagement.Application.Services;

public abstract class PartyService(IPartyRepository partyRepository) : IPartyService
{
    private readonly IPartyRepository _partyRepository = partyRepository;

    public async Task<IEnumerable<PartyModel>> GetAll(string officeId)
    {
        IEnumerable<Party> parties = await _partyRepository.GetAll(officeId);

        return parties.Select(x => new PartyModel(x));
    }

    public async Task<PartyModel?> Get(string partyId, string officeId)
    {
        Party? party = await _partyRepository.Get(partyId, officeId);

        return party is null ? null : new PartyModel(party);
    }

    public async Task<PartyModel> Create(PartyCreateModel partyModel)
    {
        if(!string.IsNullOrWhiteSpace(partyModel.IdentificationNumber) && await _partyRepository.ExistByIdentificationNumber(partyModel.OfficeId, partyModel.IdentificationNumber))
        {
            throw new ArgumentException("A party with the same identification number already exists in the office");
        }

        Party party = Party.New(partyModel.OfficeId, partyModel.FirstName, partyModel.LastName, partyModel.Address, partyModel.Description, partyModel.Phone, partyModel.IdentificationNumber);

        party = await _partyRepository.Add(party);

        return new PartyModel(party);
    }

    public async Task<PartyModel> Update(PartyModel partyModel)
    {
        if (!string.IsNullOrWhiteSpace(partyModel.IdentificationNumber) && await _partyRepository.ExistByIdentificationNumber(partyModel.OfficeId, partyModel.IdentificationNumber, partyModel.Id))
        {
            throw new ArgumentException("A party with the same identification number already exists in the office");
        }

        Party? party = await _partyRepository.Get(partyModel.Id, partyModel.OfficeId) ?? throw new ArgumentException("Party not found");

        party.SetName(partyModel.FirstName, partyModel.LastName);
        party.SetAddress(partyModel.Address);
        party.SetDescription(partyModel.Description);
        party.SetPhone(partyModel.Phone);
        party.SetIdentificationNumber(partyModel.IdentificationNumber);

        party = await _partyRepository.Update(party);

        return new PartyModel(party);
    }
}