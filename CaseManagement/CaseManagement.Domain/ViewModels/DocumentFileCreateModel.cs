namespace CaseManagement.Domain.ViewModels;

/// <summary>
/// Payload for creating a case document file descriptor.
/// </summary>
public record DocumentFileCreateModel
{
    public string CaseId { get; init; } = string.Empty;
    public string OfficeId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}
