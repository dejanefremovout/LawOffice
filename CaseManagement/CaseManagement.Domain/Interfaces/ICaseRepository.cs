using CaseManagement.Domain.Entities;
using LawOffice.Domain.Interfaces;

namespace CaseManagement.Domain.Interfaces;

/// <summary>
/// Repository contract for case aggregate persistence.
/// </summary>
public interface ICaseRepository : IRepository<Case>
{
    /// <summary>
    /// Persists a new case.
    /// </summary>
    Task<Case> Add(Case caseItem);

    /// <summary>
    /// Persists changes to an existing case.
    /// </summary>
    Task<Case> Update(Case caseItem);

    /// <summary>
    /// Gets a case by identifier and office scope.
    /// </summary>
    Task<Case?> Get(string caseId, string officeId);

    /// <summary>
    /// Gets all cases for an office.
    /// </summary>
    Task<IEnumerable<Case>> GetAll(string officeId);

    /// <summary>
    /// Gets cases by identifiers for an office.
    /// </summary>
    Task<IEnumerable<Case>> GetByIds(string officeId, IEnumerable<string> caseIds);

    /// <summary>
    /// Deletes a case by identifier.
    /// </summary>
    Task Delete(string caseId, string officeId);

    /// <summary>
    /// Gets total case count for an office.
    /// </summary>
    Task<int> GetTotalCount(string officeId);

    /// <summary>
    /// Gets active case count for an office.
    /// </summary>
    Task<int> GetActiveCount(string officeId);

    /// <summary>
    /// Gets the most recent active cases.
    /// </summary>
    Task<IEnumerable<Case>> GetLastActiveCases(string officeId, int count);
}