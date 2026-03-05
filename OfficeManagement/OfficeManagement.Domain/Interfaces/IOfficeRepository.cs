using OfficeManagement.Domain.Entities;
using LawOffice.Domain.Interfaces;

namespace OfficeManagement.Domain.Interfaces;

/// <summary>
/// Repository contract for office aggregate persistence.
/// </summary>
public interface IOfficeRepository : IRepository<Office>
{
    /// <summary>
    /// Persists a new office.
    /// </summary>
    Task<Office> Add(Office office);

    /// <summary>
    /// Persists updates to an existing office.
    /// </summary>
    Task<Office> Update(Office office);

    /// <summary>
    /// Gets an office by identifier.
    /// </summary>
    Task<Office?> Get(string officeId);

    /// <summary>
    /// Gets all offices.
    /// </summary>
    Task<IEnumerable<Office>> GetAll();
}