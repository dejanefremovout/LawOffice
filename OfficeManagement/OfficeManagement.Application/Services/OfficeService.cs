using OfficeManagement.Domain.Entities;
using OfficeManagement.Domain.Interfaces;
using OfficeManagement.Domain.ViewModels;

namespace OfficeManagement.Application.Services;

public class OfficeService(IOfficeRepository officeRepository) : IOfficeService
{
    private readonly IOfficeRepository _officeRepository = officeRepository;

    public async Task<OfficeModel?> Get(string officeId)
    {
        Office? office = await _officeRepository.Get(officeId);

        return office is null ? null : new OfficeModel(office);
    }

    public async Task<OfficeModel> Create(OfficeCreateModel officeModel)
    {
        Office office = Office.New(officeModel.Name, officeModel.Address);

        office = await _officeRepository.Add(office);

        return new OfficeModel(office);
    }

    public async Task<OfficeModel> Update(OfficeModel officeModel)
    {
        Office? office = await _officeRepository.Get(officeModel.Id) ?? throw new ArgumentException("Office not found");

        office.SetName(officeModel.Name);
        office.SetAddress(officeModel.Address);

        office = await _officeRepository.Update(office);

        return new OfficeModel(office);
    }
}
