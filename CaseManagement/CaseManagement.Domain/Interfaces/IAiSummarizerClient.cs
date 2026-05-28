namespace CaseManagement.Domain.Interfaces;

/// <summary>
/// Abstraction for AI summarization providers.
/// </summary>
public interface IAiSummarizerClient
{
    Task<string> SummarizeAsync(string content, string summaryDepth, CancellationToken cancellationToken = default);
}
