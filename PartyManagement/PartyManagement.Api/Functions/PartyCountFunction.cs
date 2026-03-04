using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PartyManagement.Api.Extensions;
using PartyManagement.Application.Services;
using PartyManagement.Domain.ViewModels;

namespace PartyManagement.Api.Functions;

public class PartyCountFunction(ILogger<PartyCountFunction> logger, IClientService clientService, IOpposingPartyService opposingPartyService)
{
    private readonly ILogger<PartyCountFunction> _logger = logger;
    private readonly IClientService _clientService = clientService;
    private readonly IOpposingPartyService _opposingPartyService = opposingPartyService;

    [Function("GetCount")]
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
            return new BadRequestObjectResult(ex.Message);
        }
    }
}