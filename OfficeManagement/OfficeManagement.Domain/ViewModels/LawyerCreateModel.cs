namespace OfficeManagement.Domain.ViewModels;

/// <summary>
/// Payload for creating a lawyer profile.
/// </summary>
public record LawyerCreateModel
{
    public required string OfficeId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
}
