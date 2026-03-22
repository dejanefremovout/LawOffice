namespace CaseManagement.Api.Messaging;

/// <summary>
/// Integration message consumed when an opposing party is deleted in PartyManagement.
/// </summary>
public sealed record OpposingPartyDeletedMessage
{
    public required string OfficeId { get; init; }
    public required string OpposingPartyId { get; init; }
    public DateTime OccurredAtUtc { get; init; }
}
