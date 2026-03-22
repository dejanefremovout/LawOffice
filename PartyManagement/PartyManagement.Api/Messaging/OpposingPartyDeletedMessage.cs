namespace PartyManagement.Api.Messaging;

/// <summary>
/// Integration event emitted when an opposing party is deleted.
/// </summary>
public sealed record OpposingPartyDeletedMessage
{
    public required string OfficeId { get; init; }
    public required string OpposingPartyId { get; init; }
    public DateTime OccurredAtUtc { get; init; }
}
