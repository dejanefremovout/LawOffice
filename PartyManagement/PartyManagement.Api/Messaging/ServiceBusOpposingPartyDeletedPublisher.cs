using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace PartyManagement.Api.Messaging;

public class ServiceBusOpposingPartyDeletedPublisher(ServiceBusSender sender) : IOpposingPartyDeletedPublisher
{
    private readonly ServiceBusSender _sender = sender;

    public async Task Publish(OpposingPartyDeletedMessage message, CancellationToken cancellationToken = default)
    {
        string payload = JsonSerializer.Serialize(message);

        ServiceBusMessage sbMessage = new(payload)
        {
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString(),
            Subject = "OpposingPartyDeleted"
        };

        sbMessage.ApplicationProperties["officeId"] = message.OfficeId;

        await _sender.SendMessageAsync(sbMessage, cancellationToken);
    }
}
