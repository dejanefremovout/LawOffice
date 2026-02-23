namespace CaseManagement.Domain.ViewModels;

public record CaseCreateModel
{
    public required string OfficeId { get; init; }
    public required IEnumerable<string> ClientIds { get; init; }
    public required IEnumerable<string> OpposingPartyIds { get; init; }
    public required string IdentificationNumber { get; init; }
    public string? Description { get; init; }
    public string? CompetentCourt { get; init; }
    public string? City { get; init; }
    public int? Year { get; init; }
    public string? Judge { get; init; }
}
