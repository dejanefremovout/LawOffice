namespace OfficeManagement.Domain.ViewModels;

public record LawyerCreateModel
{
    public required string OfficeId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
}
