using LawOffice.Domain.Entities;
using Newtonsoft.Json;

namespace OfficeManagement.Domain.Entities;

public class Office : Entity
{
    [JsonProperty("name")]
    public string Name { get; private set; }

    [JsonProperty("address")]
    public string? Address { get; private set; }

    public Office(string id, string name, string? address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name;
        Address = address;
    }

    public static Office New(string name, string? address)
    {
        return new(Guid.NewGuid().ToString(), name, address);
    }

    public void SetName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }

    public void SetAddress(string? address)
    {
        Address = address?.Trim();
    }
}
