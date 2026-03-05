using LawOffice.Domain.Entities;
using Newtonsoft.Json;

namespace OfficeManagement.Domain.Entities;

/// <summary>
/// Represents a law office tenant.
/// </summary>
public class Office : Entity
{
    [JsonProperty("name")]
    public string Name { get; private set; }

    [JsonProperty("address")]
    public string? Address { get; private set; }

    /// <summary>
    /// Initializes an office with validated values.
    /// </summary>
    public Office(string id, string name, string? address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name;
        Address = address;
    }

    /// <summary>
    /// Factory method for creating a new office.
    /// </summary>
    public static Office New(string name, string? address)
    {
        return new(Guid.NewGuid().ToString(), name, address);
    }

    /// <summary>
    /// Updates the office name.
    /// </summary>
    public void SetName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }

    /// <summary>
    /// Updates the office address.
    /// </summary>
    public void SetAddress(string? address)
    {
        Address = address?.Trim();
    }
}
