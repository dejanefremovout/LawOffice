using OfficeManagement.Domain.ViewModels;

namespace OfficeManagement.Application.Services;

public interface IOfficeService
{
    Task<IEnumerable<OfficeModel>> GetAll();
    Task<OfficeModel?> Get(string officeId);
    Task<OfficeModel> Create(OfficeCreateModel officeModel);
    Task<OfficeModel> Update(OfficeModel officeModel);
}
