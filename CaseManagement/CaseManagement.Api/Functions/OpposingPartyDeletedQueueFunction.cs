using Azure.Messaging.ServiceBus;
using CaseManagement.Api.Messaging;
using CaseManagement.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CaseManagement.Api.Functions;

/// <summary>
/// Service Bus-triggered function that reconciles case references when an opposing party is deleted.
/// </summary>
public class OpposingPartyDeletedQueueFunction(ILogger<OpposingPartyDeletedQueueFunction> logger, ICaseService caseService)
{
    private readonly ILogger<OpposingPartyDeletedQueueFunction> _logger = logger;
    private readonly ICaseService _caseService = caseService;

    [Function("HandleOpposingPartyDeleted")]
    public async Task Run([ServiceBusTrigger("%OpposingPartyDeletedQueueName%", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message)
    {
        OpposingPartyDeletedMessage? deletedMessage;

        try
        {
            deletedMessage = JsonSerializer.Deserialize<OpposingPartyDeletedMessage>(message.Body.ToString());
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid opposing-party-deleted message payload. MessageId: {MessageId}", message.MessageId);
            throw;
        }

        if (deletedMessage is null || string.IsNullOrWhiteSpace(deletedMessage.OfficeId) || string.IsNullOrWhiteSpace(deletedMessage.OpposingPartyId))
        {
            throw new ArgumentException("OpposingPartyDeleted message is missing required fields.");
        }

        int updatedCases = await _caseService.RemoveOpposingPartyReferences(deletedMessage.OfficeId, deletedMessage.OpposingPartyId);

        _logger.LogInformation(
            "Processed OpposingPartyDeleted message {MessageId}. OfficeId: {OfficeId}, OpposingPartyId: {OpposingPartyId}, UpdatedCases: {UpdatedCases}",
            message.MessageId,
            deletedMessage.OfficeId,
            deletedMessage.OpposingPartyId,
            updatedCases);
    }
}
