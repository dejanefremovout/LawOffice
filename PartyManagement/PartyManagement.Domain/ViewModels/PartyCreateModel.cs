namespace PartyManagement.Domain.ViewModels;

/// <summary>
/// Payload for creating a party record.
/// </summary>
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
