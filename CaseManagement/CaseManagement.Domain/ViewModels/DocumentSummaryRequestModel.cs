namespace CaseManagement.Domain.ViewModels;

/// <summary>
/// Request payload for generating a document summary.
/// </summary>
public record DocumentSummaryRequestModel
{
    public string SummaryDepth { get; init; } = "short";
}
