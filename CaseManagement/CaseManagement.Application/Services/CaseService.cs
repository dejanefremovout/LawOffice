using CaseManagement.Domain.Entities;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Domain.ViewModels;

namespace CaseManagement.Application.Services;

public class CaseService(ICaseRepository caseRepository) : ICaseService
{
    private readonly ICaseRepository _caseRepository = caseRepository;

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
}