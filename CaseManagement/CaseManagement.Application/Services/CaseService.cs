using CaseManagement.Domain.Entities;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Domain.ViewModels;

namespace CaseManagement.Application.Services;

public class CaseService(ICaseRepository caseRepository, IHearingRepository hearingRepository) : ICaseService
{
    private readonly ICaseRepository _caseRepository = caseRepository;
    private readonly IHearingRepository _hearingRepository = hearingRepository;

    public async Task<CaseModel?> Get(string caseId, string officeId)
    {
        Case? caseItem = await _caseRepository.Get(caseId, officeId);

        return caseItem is null ? null : new CaseModel(caseItem);
    }

    public async Task<IEnumerable<CaseModel>> GetAll(string officeId)
    {
        IEnumerable<Case> cases = await _caseRepository.GetAll(officeId);

        return cases.Select(x => new CaseModel(x));
    }

    public async Task<CaseModel> Create(CaseCreateModel caseModel)
    {
        Case caseItem = Case.New(caseModel.OfficeId, caseModel.ClientIds, caseModel.OpposingPartyIds, caseModel.IdentificationNumber, caseModel.Description, caseModel.CompetentCourt, caseModel.City, caseModel.Year, caseModel.Judge);

        caseItem = await _caseRepository.Add(caseItem);

        return new CaseModel(caseItem);
    }

    public async Task<CaseModel> Update(CaseModel caseModel)
    {
        Case? caseItem = await _caseRepository.Get(caseModel.Id, caseModel.OfficeId) ?? throw new ArgumentException("Case not found");

        caseItem.SetClientIds(caseModel.ClientIds);
        caseItem.SetOpposingPartyIds(caseModel.OpposingPartyIds);
        caseItem.SetIdentificationNumber(caseModel.IdentificationNumber);
        caseItem.UpdateFields(caseModel.Description, caseModel.Active, caseModel.CompetentCourt, caseModel.City, caseModel.Year, caseModel.Judge);

        caseItem = await _caseRepository.Update(caseItem);

        return new CaseModel(caseItem);
    }

    public async Task Delete(string caseId, string officeId)
    {
        _ = await _caseRepository.Get(caseId, officeId) ?? throw new ArgumentException("Case not found");

        await _caseRepository.Delete(caseId, officeId);
    }

    public async Task<CaseCountModel> GetCount(string officeId)
    {
        return new CaseCountModel
        {
            TotalCases = await _caseRepository.GetTotalCount(officeId),
            ActiveCases = await _caseRepository.GetActiveCount(officeId)
        };
    }

    public async Task<IEnumerable<CaseModel>> GetLastActiveCases(string officeId, int count)
    {
        IEnumerable<Case> cases = await _caseRepository.GetLastActiveCases(officeId, count);

        return cases.Select(x => new CaseModel(x));
    }

    public async Task<IEnumerable<CaseHearingModel>> GetCasesWithUpcomingHearings(string officeId, int count)
    {
        IEnumerable<Hearing> hearings = await _hearingRepository.GetUpcomingHearings(officeId, count);
        if (!hearings.Any())
        {
            return [];
        }

        IEnumerable<Case> cases = await _caseRepository.GetByIds(officeId, hearings.Select(x => x.CaseId));

        // Pair each hearing with its case to return a single projection for dashboard-like views.
        return hearings.Select(hearing => new CaseHearingModel(cases.First(caseItem => caseItem.Id == hearing.CaseId), hearing));
    }
}