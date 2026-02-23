using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PartyManagement.Application.Services;
using PartyManagement.Domain.ViewModels;

namespace PartyManagement.Api.Functions;

public class OpposingPartyFunction(ILogger<OpposingPartyFunction> logger, IOpposingPartyService opposingPartyService)
{
    private readonly ILogger<OpposingPartyFunction> _logger = logger;
    private readonly IOpposingPartyService _opposingPartyService = opposingPartyService;

    [Function("GetOpposingParty")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "opposingParty/{officeId}/{opposingPartyId}")] HttpRequest req, string officeId, string opposingPartyId)
    {
        try
        {
            if (string.IsNullOrEmpty(officeId))
            {
                return new BadRequestObjectResult("officeId route parameter is required.");
            }

            if (string.IsNullOrEmpty(opposingPartyId))
            {
                return new BadRequestObjectResult("opposingPartyId route parameter is required.");
            }

            PartyModel? result = await _opposingPartyService.Get(opposingPartyId, officeId);

            return result == null ? new NotFoundObjectResult("Opposing party not found.") : new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving opposing party.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opposing party.");
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("GetAllOpposingParties")]
    public async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = "opposingParty/{officeId}")] HttpRequest req, string officeId)
    {
        try
        {
            if (string.IsNullOrEmpty(officeId))
            {
                return new BadRequestObjectResult("officeId route parameter is required.");
            }

            IEnumerable<PartyModel> result = await _opposingPartyService.GetAll(officeId);

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving opposing parties.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opposing parties.");
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("PostOpposingParty")]
    public async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Function, "post", Route = "opposingParty")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            PartyCreateModel? opposingPartyModel = JsonConvert.DeserializeObject<PartyCreateModel>(requestBody);

            if (opposingPartyModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            PartyModel result = await _opposingPartyService.Create(opposingPartyModel);
            return new CreatedResult($"/opposingParty/{result.OfficeId}/{result.Id}", result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when creating opposing party.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating opposing party.");
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("PutOpposingParty")]
    public async Task<IActionResult> Put([HttpTrigger(AuthorizationLevel.Function, "put", Route = "opposingParty")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            PartyModel? opposingPartyModel = JsonConvert.DeserializeObject<PartyModel>(requestBody);

            if (opposingPartyModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            PartyModel result = await _opposingPartyService.Update(opposingPartyModel);
            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when updating opposing party.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating opposing party.");
            return new BadRequestObjectResult(ex.Message);
        }
    }
}