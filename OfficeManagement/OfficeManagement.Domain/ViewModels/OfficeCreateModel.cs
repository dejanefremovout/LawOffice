namespace OfficeManagement.Domain.ViewModels;

/// <summary>
/// Payload for creating a new office.
/// </summary>
public record OfficeCreateModel
{
    public required string Name { get; init; }
    public string? Address { get; init; }
}
