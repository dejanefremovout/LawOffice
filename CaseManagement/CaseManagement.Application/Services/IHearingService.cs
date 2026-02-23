using CaseManagement.Domain.ViewModels;

namespace CaseManagement.Application.Services;

public interface IHearingService
{
    Task<HearingModel?> Get(string hearingId, string officeId);
    Task<IEnumerable<HearingModel>> GetAll(string caseId, string officeId);
    Task<HearingModel> Create(HearingCreateModel hearingModel);
    Task<HearingModel> Update(HearingModel hearingModel);
    Task Delete(string caseId, string officeId);
}
