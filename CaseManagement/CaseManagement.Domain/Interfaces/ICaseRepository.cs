using CaseManagement.Domain.Entities;
using LawOffice.Domain.Interfaces;

namespace CaseManagement.Domain.Interfaces;

public interface ICaseRepository : IRepository<Case>
{
    Task<Case> Add(Case caseItem);
    Task<Case> Update(Case caseItem);
    Task<Case?> Get(string caseId, string officeId);
    Task<IEnumerable<Case>> GetAll(string officeId);
    Task<IEnumerable<Case>> GetByIds(string officeId, IEnumerable<string> caseIds);
    Task Delete(string caseId, string officeId);
    Task<int> GetTotalCount(string officeId);
    Task<int> GetActiveCount(string officeId);
    Task<IEnumerable<Case>> GetLastActiveCases(string officeId, int count);
}