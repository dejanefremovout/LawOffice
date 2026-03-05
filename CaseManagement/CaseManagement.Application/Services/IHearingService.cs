using CaseManagement.Domain.ViewModels;

namespace CaseManagement.Application.Services;

/// <summary>
/// Application service for hearing scheduling and maintenance.
/// </summary>
public interface IHearingService
{
    /// <summary>
    /// Gets a hearing by identifier.
    /// </summary>
    Task<HearingModel?> Get(string hearingId, string officeId);

    /// <summary>
    /// Gets all hearings for a case.
    /// </summary>
    Task<IEnumerable<HearingModel>> GetAll(string caseId, string officeId);

    /// <summary>
    /// Creates a hearing.
    /// </summary>
    Task<HearingModel> Create(HearingCreateModel hearingModel);

    /// <summary>
    /// Updates a hearing.
    /// </summary>
    Task<HearingModel> Update(HearingModel hearingModel);

    /// <summary>
    /// Deletes a hearing by identifier.
    /// </summary>
    Task Delete(string caseId, string officeId);
}
