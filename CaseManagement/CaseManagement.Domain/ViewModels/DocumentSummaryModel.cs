namespace CaseManagement.Domain.ViewModels;

/// <summary>
/// Structured AI summary response for a document.
/// </summary>
public record DocumentSummaryModel
{
    public string DocumentFileId { get; init; } = string.Empty;
    public string SummaryDepth { get; init; } = string.Empty;
    public string ShortSummary { get; init; } = string.Empty;
    public IEnumerable<string> KeyPoints { get; init; } = [];
    public IEnumerable<string> ActionItems { get; init; } = [];
}
