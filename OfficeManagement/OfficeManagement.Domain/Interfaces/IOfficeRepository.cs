using OfficeManagement.Domain.Entities;
using LawOffice.Domain.Interfaces;

namespace OfficeManagement.Domain.Interfaces;

public interface IOfficeRepository : IRepository<Office>
{
    Task<Office> Add(Office office);

    Task<Office> Update(Office office);

    Task<Office?> Get(string officeId);

    Task<IEnumerable<Office>> GetAll();
}