using LawOffice.Domain.Interfaces;
using OfficeManagement.Domain.Entities;

namespace OfficeManagement.Domain.Interfaces;

public interface ILawyerRepository : IRepository<Lawyer>
{
    Task<Lawyer> Add(Lawyer lawyer);

    Task<Lawyer> Update(Lawyer lawyer);

    Task<Lawyer?> Get(string lawyerId, string officeId);

    Task<IEnumerable<Lawyer>> GetAll(string officeId);

    Task<Lawyer?> GetByEmail(string email);
}