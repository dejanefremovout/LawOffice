namespace PartyManagement.Domain.ViewModels;

public record PartyCreateModel
{
    public required string OfficeId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? Address { get; init; }
    public string? Description { get; init; }
    public string? Phone { get; init; }
    public string? IdentificationNumber { get; init; }
}
