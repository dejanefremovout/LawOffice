using CaseManagement.Domain.Entities;

namespace CaseManagement.Domain.Interfaces;

/// <summary>
/// Repository contract for hearing aggregate persistence.
/// </summary>
public interface IHearingRepository : IRepository<Hearing>
{
    /// <summary>
    /// Persists a new hearing.
    /// </summary>
    Task<Hearing> Add(Hearing hearing);

    /// <summary>
    /// Persists updates to an existing hearing.
    /// </summary>
    Task<Hearing> Update(Hearing hearing);

    /// <summary>
    /// Gets a hearing by identifier and office scope.
    /// </summary>
    Task<Hearing?> Get(string hearingId, string officeId);

    /// <summary>
    /// Gets all hearings for a case.
    /// </summary>
    Task<IEnumerable<Hearing>> GetAll(string caseId, string officeId);

    /// <summary>
    /// Gets upcoming hearings for an office.
    /// </summary>
    Task<IEnumerable<Hearing>> GetUpcomingHearings(string officeId, int count);

    /// <summary>
    /// Deletes a hearing by identifier.
    /// </summary>
    Task Delete(string hearingId, string officeId);
}