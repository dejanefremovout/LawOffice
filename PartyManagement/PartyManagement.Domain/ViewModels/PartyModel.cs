using PartyManagement.Domain.Entities;

namespace PartyManagement.Domain.ViewModels;

public record PartyModel
{
    public PartyModel()
    {
    }

    public PartyModel(Party party)
    {
        Id = party.Id;
        OfficeId = party.OfficeId;
        FirstName = party.FirstName;
        LastName = party.LastName;
        Address = party.Address;
        Description = party.Description;
        Phone = party.Phone;
        IdentificationNumber = party.IdentificationNumber;
    }

    public string Id { get; init; } = string.Empty;
    public string OfficeId { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? Description { get; init; }
    public string? Phone { get; init; }
    public string? IdentificationNumber { get; init; }
}
