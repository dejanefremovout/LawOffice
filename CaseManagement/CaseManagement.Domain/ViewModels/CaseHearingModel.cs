using CaseManagement.Domain.Entities;

namespace CaseManagement.Domain.ViewModels;

public record CaseHearingModel
{
    public CaseHearingModel()
    {
    }

    public CaseHearingModel(Case caseItem, Hearing hearing)
    {
        Id = caseItem.Id;
        IdentificationNumber = caseItem.IdentificationNumber;
        Date = hearing.Date;
    }

    public string Id { get; init; } = string.Empty;
    public string IdentificationNumber { get; init; } = string.Empty;
    public DateTime Date { get; init; }
}
