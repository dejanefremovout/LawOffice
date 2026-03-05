using CaseManagement.Domain.Entities;

namespace CaseManagement.Domain.ViewModels;

/// <summary>
/// Case DTO returned by application services and APIs.
/// </summary>
public record CaseModel
{
    public CaseModel()
    {
    }

    public CaseModel(Case caseItem)
    {
        Id = caseItem.Id;
        OfficeId = caseItem.OfficeId;
        ClientIds = caseItem.ClientIds;
        OpposingPartyIds = caseItem.OpposingPartyIds;
        IdentificationNumber = caseItem.IdentificationNumber;
        Description = caseItem.Description;
        Active = caseItem.Active;
        CompetentCourt = caseItem.CompetentCourt;
        City = caseItem.City;
        Year = caseItem.Year;
        Judge = caseItem.Judge;
    }

    public string Id { get; init; } = string.Empty;
    public string OfficeId { get; init; } = string.Empty;
    public IEnumerable<string> ClientIds { get; init; } = [];
    public IEnumerable<string> OpposingPartyIds { get; init; } = [];
    public string IdentificationNumber { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool Active { get; init; }
    public string? CompetentCourt { get; init; }
    public string? City { get; init; }
    public int? Year { get; init; }
    public string? Judge { get; init; }
}
