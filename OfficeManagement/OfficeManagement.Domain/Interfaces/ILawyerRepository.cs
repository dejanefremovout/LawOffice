using OfficeManagement.Domain.Entities;

namespace OfficeManagement.Domain.Interfaces;

/// <summary>
/// Repository contract for lawyer aggregate persistence.
/// </summary>
public interface ILawyerRepository : IRepository<Lawyer>
{
    /// <summary>
    /// Persists a new lawyer.
    /// </summary>
    Task<Lawyer> Add(Lawyer lawyer);

    /// <summary>
    /// Persists updates to an existing lawyer.
    /// </summary>
    Task<Lawyer> Update(Lawyer lawyer);

    /// <summary>
    /// Gets a lawyer by identifier and office scope.
    /// </summary>
    Task<Lawyer?> Get(string lawyerId, string officeId);

    /// <summary>
    /// Gets all lawyers for an office.
    /// </summary>
    Task<IEnumerable<Lawyer>> GetAll(string officeId);

    /// <summary>
    /// Gets a lawyer by email.
    /// </summary>
    Task<Lawyer?> GetByEmail(string email);
}