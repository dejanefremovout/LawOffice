namespace OfficeManagement.Domain.ViewModels;

public record OfficeCreateModel
{
    public required string Name { get; init; }
    public string? Address { get; init; }
}
