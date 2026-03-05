using OfficeManagement.Domain.ViewModels;

namespace OfficeManagement.Application.Services;

/// <summary>
/// Application service for office lifecycle operations.
/// </summary>
public interface IOfficeService
{
    /// <summary>
    /// Gets all offices.
    /// </summary>
    Task<IEnumerable<OfficeModel>> GetAll();

    /// <summary>
    /// Gets an office by identifier.
    /// </summary>
    Task<OfficeModel?> Get(string officeId);

    /// <summary>
    /// Creates a new office.
    /// </summary>
    Task<OfficeModel> Create(OfficeCreateModel officeModel);

    /// <summary>
    /// Updates an existing office.
    /// </summary>
    Task<OfficeModel> Update(OfficeModel officeModel);
}
