namespace PartyManagement.Domain.ViewModels;

public record PartyCountModel
{
    public int ClientsCount { get; init; }
    public int OpposingPartiesCount { get; init; }
}