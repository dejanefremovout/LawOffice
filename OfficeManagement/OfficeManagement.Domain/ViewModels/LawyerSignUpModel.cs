using OfficeManagement.Domain.Entities;

namespace OfficeManagement.Domain.ViewModels;

public record LawyerSignUpModel
{
    public LawyerSignUpModel()
    {
    }

    public LawyerSignUpModel(Lawyer lawyer)
    {
        Id = lawyer.Id;
        OfficeId = lawyer.OfficeId;
        Email = lawyer.Email;
        InvitationCode = lawyer.InvitationCode;
    }

    public string Id { get; init; } = string.Empty;
    public string OfficeId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? InvitationCode { get; init; } = string.Empty;
}
