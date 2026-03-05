namespace PartyManagement.Domain.ViewModels;

/// <summary>
/// Aggregate counts for party entities in an office.
/// </summary>
public record PartyCountModel
{
    public int ClientsCount { get; init; }
    public int OpposingPartiesCount { get; init; }
}