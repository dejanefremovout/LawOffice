namespace CaseManagement.Domain.ViewModels;

public record HearingCreateModel
{
    public string CaseId { get; init; } = string.Empty;
    public string OfficeId { get; init; } = string.Empty;
    public string? Courtroom { get; init; }
    public string? Description { get; init; }
    public DateTime Date { get; init; }
    public bool Held { get; init; }
}
