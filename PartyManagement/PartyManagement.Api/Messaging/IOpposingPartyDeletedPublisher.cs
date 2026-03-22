namespace PartyManagement.Api.Messaging;

public interface IOpposingPartyDeletedPublisher
{
    Task Publish(OpposingPartyDeletedMessage message, CancellationToken cancellationToken = default);
}
