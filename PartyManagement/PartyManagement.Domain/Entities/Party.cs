using LawOffice.Domain.Entities;
using Newtonsoft.Json;

namespace PartyManagement.Domain.Entities;

public class Party : Entity
{
    [JsonProperty("officeId")]
    public string OfficeId { get; private set; }

    [JsonProperty("firstName")]
    public string FirstName { get; private set; }

    [JsonProperty("lastName")]
    public string LastName { get; private set; }

    [JsonProperty("address")]
    public string? Address { get; private set; }

    [JsonProperty("description")]
    public string? Description { get; private set; }

    [JsonProperty("phone")]
    public string? Phone { get; private set; }

    [JsonProperty("identificationNumber")]
    public string? IdentificationNumber { get; private set; }

    public Party(string id, string officeId, string firstName, string lastName, string? address, string? description, string? phone, string? identificationNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        Id = id;
        OfficeId = officeId;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Address = address?.Trim();
        Description = description?.Trim();
        Phone = phone?.Trim();
        IdentificationNumber = identificationNumber?.Trim();
    }

    public static Party New(string officeId, string firstName, string lastName, string? address, string? description, string? phone, string? identificationNumber)
    {
        return new(Guid.NewGuid().ToString(), officeId, firstName, lastName, address, description, phone, identificationNumber);
    }

    public void SetName(string firstName, string lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public void SetAddress(string? address)
    {
        Address = address?.Trim();
    }

    public void SetDescription(string? description)
    {
        Description = description?.Trim();
    }

    public void SetPhone(string? phone)
    {
        Phone = phone?.Trim();
    }

    public void SetIdentificationNumber(string? identificationNumber)
    {
        IdentificationNumber = identificationNumber?.Trim();
    }
}
