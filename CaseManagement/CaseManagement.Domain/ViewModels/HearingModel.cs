using CaseManagement.Domain.Entities;

namespace CaseManagement.Domain.ViewModels;

public record HearingModel
{
    public HearingModel()
    {
    }

    public HearingModel(Hearing hearing)
    {
        Id = hearing.Id;
        OfficeId = hearing.OfficeId;
        CaseId = hearing.Id;
        Courtroom = hearing.Courtroom;
        Description = hearing.Description;
        Date = hearing.Date;
        Held = hearing.Held;
    }

    public string Id { get; init; } = string.Empty;
    public string CaseId { get; init; } = string.Empty;
    public string OfficeId { get; init; } = string.Empty;
    public string? Courtroom { get; init; }
    public string? Description { get; init; }
    public DateTime Date { get; init; }
    public bool Held { get; init; }
}
