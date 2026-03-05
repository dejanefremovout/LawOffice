using CaseManagement.Domain.ViewModels;

namespace CaseManagement.Application.Services;

/// <summary>
/// Application service for case lifecycle operations.
/// </summary>
public interface ICaseService
{
    /// <summary>
    /// Gets a case by identifier within an office scope.
    /// </summary>
    Task<CaseModel?> Get(string caseId, string officeId);

    /// <summary>
    /// Gets all cases for an office.
    /// </summary>
    Task<IEnumerable<CaseModel>> GetAll(string officeId);

    /// <summary>
    /// Creates a new case.
    /// </summary>
    Task<CaseModel> Create(CaseCreateModel caseModel);

    /// <summary>
    /// Updates an existing case.
    /// </summary>
    Task<CaseModel> Update(CaseModel caseModel);

    /// <summary>
    /// Deletes a case by identifier.
    /// </summary>
    Task Delete(string caseId, string officeId);

    /// <summary>
    /// Returns total and active case counts.
    /// </summary>
    Task<CaseCountModel> GetCount(string officeId);

    /// <summary>
    /// Gets the last active cases ordered by recency.
    /// </summary>
    Task<IEnumerable<CaseModel>> GetLastActiveCases(string officeId, int count);

    /// <summary>
    /// Gets cases that have upcoming hearings.
    /// </summary>
    Task<IEnumerable<CaseHearingModel>> GetCasesWithUpcomingHearings(string officeId, int count);
}
