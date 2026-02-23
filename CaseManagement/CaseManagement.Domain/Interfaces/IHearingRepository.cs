using CaseManagement.Domain.Entities;
using LawOffice.Domain.Interfaces;

namespace CaseManagement.Domain.Interfaces;

public interface IHearingRepository : IRepository<Hearing>
{
    Task<Hearing> Add(Hearing hearing);
    Task<Hearing> Update(Hearing hearing);
    Task<Hearing?> Get(string hearingId, string officeId);
    Task<IEnumerable<Hearing>> GetAll(string caseId, string officeId);
    Task Delete(string hearingId, string officeId);
}