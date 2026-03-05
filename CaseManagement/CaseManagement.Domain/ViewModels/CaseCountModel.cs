namespace CaseManagement.Domain.ViewModels;

/// <summary>
/// Aggregate case counters for an office.
/// </summary>
public record CaseCountModel
{
    public int TotalCases { get; init; }
    public int ActiveCases { get; init; }
}
