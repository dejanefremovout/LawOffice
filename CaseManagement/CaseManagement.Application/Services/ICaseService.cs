using CaseManagement.Domain.ViewModels;

namespace CaseManagement.Application.Services;

public interface ICaseService
{
    Task<CaseModel?> Get(string caseId, string officeId);
    Task<IEnumerable<CaseModel>> GetAll(string officeId);
    Task<CaseModel> Create(CaseCreateModel caseModel);
    Task<CaseModel> Update(CaseModel caseModel);
    Task Delete(string caseId, string officeId);
    Task<CaseCountModel> GetCount(string officeId);
    Task<IEnumerable<CaseModel>> GetLastActiveCases(string officeId, int count);
    Task<IEnumerable<CaseHearingModel>> GetCasesWithUpcomingHearings(string officeId, int count);
}
