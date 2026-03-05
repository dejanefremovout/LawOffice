using OfficeManagement.Domain.Entities;

namespace OfficeManagement.Domain.ViewModels;

/// <summary>
/// Office DTO returned by API and application services.
/// </summary>
public record OfficeModel
{
    public OfficeModel()
    {
    }

    public OfficeModel(Office office)
    {
        Id = office.Id;
        Name = office.Name;
        Address = office.Address;
    }

    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Address { get; init; }
}
