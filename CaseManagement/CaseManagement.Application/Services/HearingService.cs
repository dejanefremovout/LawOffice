using CaseManagement.Domain.Entities;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Domain.ViewModels;

namespace CaseManagement.Application.Services;

public class HearingService(IHearingRepository hearingRepository,
    ICaseRepository caseRepository) : IHearingService
{
    private readonly IHearingRepository _hearingRepository = hearingRepository;
    private readonly ICaseRepository _caseRepository = caseRepository;

    public async Task<HearingModel?> Get(string hearingId, string officeId)
    {
        Hearing? hearingItem = await _hearingRepository.Get(hearingId, officeId);

        return hearingItem is null ? null : new HearingModel(hearingItem);
    }

    public async Task<IEnumerable<HearingModel>> GetAll(string caseId, string officeId)
    {
        _ = await _caseRepository.Get(caseId, officeId) ?? throw new ArgumentException("Case not found");

        IEnumerable<Hearing> hearings = await _hearingRepository.GetAll(caseId, officeId);

        return hearings.Select(x => new HearingModel(x));
    }

    public async Task<HearingModel> Create(HearingCreateModel hearingModel)
    {
        _ = await _caseRepository.Get(hearingModel.CaseId, hearingModel.OfficeId) ?? throw new ArgumentException("Case not found");

        Hearing hearingItem = Hearing.New(hearingModel.OfficeId, hearingModel.CaseId, hearingModel.Courtroom, hearingModel.Description, hearingModel.Date);

        hearingItem = await _hearingRepository.Add(hearingItem);

        return new HearingModel(hearingItem);
    }

    public async Task<HearingModel> Update(HearingModel hearingModel)
    {
        Hearing? hearingItem = await _hearingRepository.Get(hearingModel.Id, hearingModel.OfficeId) ?? throw new ArgumentException("Hearing not found");

        hearingItem.SetCourtroom(hearingModel.Courtroom);
        hearingItem.SetDescription(hearingModel.Description);
        hearingItem.SetDate(hearingModel.Date);
        hearingItem.SetHeld(hearingModel.Held);

        hearingItem = await _hearingRepository.Update(hearingItem);

        return new HearingModel(hearingItem);
    }

    public async Task Delete(string hearingId, string officeId)
    {
        _ = await _hearingRepository.Get(hearingId, officeId) ?? throw new ArgumentException("Hearing not found");

        await _hearingRepository.Delete(hearingId, officeId);
    }
}