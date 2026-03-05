using OfficeManagement.Domain.ViewModels;

namespace OfficeManagement.Application.Services;

/// <summary>
/// Application service for lawyer operations.
/// </summary>
public interface ILawyerService
{
    /// <summary>
    /// Gets a lawyer by identifier within an office.
    /// </summary>
    Task<LawyerModel?> Get(string lawyerId, string officeId);

    /// <summary>
    /// Gets all lawyers for an office.
    /// </summary>
    Task<IEnumerable<LawyerModel>> GetAll(string officeId);

    /// <summary>
    /// Creates a lawyer profile.
    /// </summary>
    Task<LawyerModel> Create(LawyerCreateModel lawyerModel);

    /// <summary>
    /// Updates a lawyer profile.
    /// </summary>
    Task<LawyerModel> Update(LawyerModel lawyerModel);

    /// <summary>
    /// Validates an invitation code for the specified email.
    /// </summary>
    Task<bool> ValidateInvitationCode(string lawyerEmail, string invitationCode);

    /// <summary>
    /// Checks whether a user account with the email exists.
    /// </summary>
    Task<bool> UserWithEmailExist(string lawyerEmail);

    /// <summary>
    /// Gets a lawyer by email.
    /// </summary>
    Task<LawyerModel?> GetByEmail(string lawyerEmail);
}
