using OfficeManagement.Domain.Entities;

namespace OfficeManagement.Domain.ViewModels;

public record LawyerModel
{
    public LawyerModel()
    {
    }

    public LawyerModel(Lawyer lawyer)
    {
        Id = lawyer.Id;
        OfficeId = lawyer.OfficeId;
        Active = lawyer.Active;
        FirstName = lawyer.FirstName;
        LastName = lawyer.LastName;
        Email = lawyer.Email;
        InvitationCode = lawyer.InvitationCode;
    }

    public string Id { get; init; } = string.Empty;
    public string OfficeId { get; init; } = string.Empty;
    public bool Active { get; init; } = false;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? InvitationCode { get; init; }
}
