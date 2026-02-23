namespace CaseManagement.Domain.ViewModels;

public record DocumentFileCreateModel
{
    public string CaseId { get; init; } = string.Empty;
    public string OfficeId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}
