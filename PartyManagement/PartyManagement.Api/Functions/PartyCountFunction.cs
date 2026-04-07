using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using PartyManagement.Api.Extensions;
using PartyManagement.Application.Services;
using PartyManagement.Domain.ViewModels;

namespace PartyManagement.Api.Functions;

/// <summary>
/// HTTP-triggered operations for party aggregate metrics.
/// </summary>
public class PartyCountFunction(ILogger<PartyCountFunction> logger, IClientService clientService, IOpposingPartyService opposingPartyService)
{
    private readonly ILogger<PartyCountFunction> _logger = logger;
    private readonly IClientService _clientService = clientService;
    private readonly IOpposingPartyService _opposingPartyService = opposingPartyService;

    /// <summary>
    /// Gets client and opposing party counts for the current office.
    /// </summary>
    [Function("GetCount")]
    [OpenApiOperation(operationId: "getPartyCount", tags: ["Party"], Summary = "Get party count aggregates")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PartyCountModel), Description = "Party count aggregates")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "party/count")] HttpRequest req)
    {
        try
        {
            var officeId = req.GetOfficeId();

            PartyCountModel result = new PartyCountModel
            {
                ClientsCount = await _clientService.GetCount(officeId),
                OpposingPartiesCount = await _opposingPartyService.GetCount(officeId)
            };

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving party count.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving party count.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}