namespace CaseManagement.Domain.ViewModels;

public record CaseCountModel
{
    public int TotalCases { get; init; }
    public int ActiveCases { get; init; }
}
