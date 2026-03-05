using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PartyManagement.Api.Extensions;
using PartyManagement.Application.Services;
using PartyManagement.Domain.ViewModels;

namespace PartyManagement.Api.Functions;

/// <summary>
/// HTTP-triggered operations for opposing party endpoints.
/// </summary>
public class OpposingPartyFunction(ILogger<OpposingPartyFunction> logger, IOpposingPartyService opposingPartyService)
{
    private readonly ILogger<OpposingPartyFunction> _logger = logger;
    private readonly IOpposingPartyService _opposingPartyService = opposingPartyService;

    /// <summary>
    /// Gets an opposing party by identifier.
    /// </summary>
    [Function("GetOpposingParty")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "opposingParty/{opposingPartyId}")] HttpRequest req, string opposingPartyId)
    {
        try
        {
            var officeId = req.GetOfficeId();

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

    /// <summary>
    /// Gets all opposing parties for the current office.
    /// </summary>
    [Function("GetAllOpposingParties")]
    public async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = "opposingParty")] HttpRequest req)
    {
        try
        {
            var officeId = req.GetOfficeId();

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

    /// <summary>
    /// Creates an opposing party.
    /// </summary>
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

            opposingPartyModel = opposingPartyModel with { OfficeId = req.GetOfficeId() };

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

    /// <summary>
    /// Updates an opposing party.
    /// </summary>
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

            opposingPartyModel = opposingPartyModel with { OfficeId = req.GetOfficeId() };

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