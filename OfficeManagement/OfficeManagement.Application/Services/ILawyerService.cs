using OfficeManagement.Domain.ViewModels;

namespace OfficeManagement.Application.Services;

public interface ILawyerService
{
    Task<LawyerModel?> Get(string lawyerId, string officeId);
    Task<IEnumerable<LawyerModel>> GetAll(string officeId);
    Task<LawyerModel> Create(LawyerCreateModel lawyerModel);
    Task<LawyerModel> Update(LawyerModel lawyerModel);
    Task<bool> ValidateInvitationCode(string lawyerEmail, string invitationCode);
}
