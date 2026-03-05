using LawOffice.Domain.Entities;
using Newtonsoft.Json;

namespace PartyManagement.Domain.Entities;

/// <summary>
/// Represents a person or organization participating in legal cases.
/// </summary>
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

    /// <summary>
    /// Initializes a party with validated values.
    /// </summary>
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

    /// <summary>
    /// Factory method for creating a new party.
    /// </summary>
    public static Party New(string officeId, string firstName, string lastName, string? address, string? description, string? phone, string? identificationNumber)
    {
        return new(Guid.NewGuid().ToString(), officeId, firstName, lastName, address, description, phone, identificationNumber);
    }

    /// <summary>
    /// Updates first and last name.
    /// </summary>
    public void SetName(string firstName, string lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    /// <summary>
    /// Updates address information.
    /// </summary>
    public void SetAddress(string? address)
    {
        Address = address?.Trim();
    }

    /// <summary>
    /// Updates descriptive notes.
    /// </summary>
    public void SetDescription(string? description)
    {
        Description = description?.Trim();
    }

    /// <summary>
    /// Updates phone information.
    /// </summary>
    public void SetPhone(string? phone)
    {
        Phone = phone?.Trim();
    }

    /// <summary>
    /// Updates the party identification number.
    /// </summary>
    public void SetIdentificationNumber(string? identificationNumber)
    {
        IdentificationNumber = identificationNumber?.Trim();
    }
}
