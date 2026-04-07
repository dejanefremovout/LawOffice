using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using PartyManagement.Api.Extensions;
using PartyManagement.Api.Messaging;
using PartyManagement.Application.Services;
using PartyManagement.Domain.ViewModels;

namespace PartyManagement.Api.Functions;

/// <summary>
/// HTTP-triggered operations for opposing party endpoints.
/// </summary>
public class OpposingPartyFunction(ILogger<OpposingPartyFunction> logger, IOpposingPartyService opposingPartyService, IOpposingPartyDeletedPublisher opposingPartyDeletedPublisher)
{
    private readonly ILogger<OpposingPartyFunction> _logger = logger;
    private readonly IOpposingPartyService _opposingPartyService = opposingPartyService;
    private readonly IOpposingPartyDeletedPublisher _opposingPartyDeletedPublisher = opposingPartyDeletedPublisher;

    /// <summary>
    /// Gets an opposing party by identifier.
    /// </summary>
    [Function("GetOpposingParty")]
    [OpenApiOperation(operationId: "getOpposingParty", tags: ["OpposingParty"], Summary = "Get an opposing party by ID")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiParameter(name: "opposingPartyId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Opposing party identifier")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PartyModel), Description = "The requested opposing party")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/plain", bodyType: typeof(string), Description = "Opposing party not found")]
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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Gets all opposing parties for the current office.
    /// </summary>
    [Function("GetAllOpposingParties")]
    [OpenApiOperation(operationId: "getAllOpposingParties", tags: ["OpposingParty"], Summary = "Get all opposing parties")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<PartyModel>), Description = "List of opposing parties")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Creates an opposing party.
    /// </summary>
    [Function("PostOpposingParty")]
    [OpenApiOperation(operationId: "createOpposingParty", tags: ["OpposingParty"], Summary = "Create an opposing party")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(PartyCreateModel), Required = true, Description = "Opposing party creation payload")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(PartyModel), Description = "Created opposing party")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Updates an opposing party.
    /// </summary>
    [Function("PutOpposingParty")]
    [OpenApiOperation(operationId: "updateOpposingParty", tags: ["OpposingParty"], Summary = "Update an opposing party")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(PartyModel), Required = true, Description = "Updated opposing party payload")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PartyModel), Description = "Updated opposing party")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Deletes an opposing party by identifier and emits an integration event.
    /// </summary>
    [Function("DeleteOpposingParty")]
    [OpenApiOperation(operationId: "deleteOpposingParty", tags: ["OpposingParty"], Summary = "Delete an opposing party")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiParameter(name: "opposingPartyId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Opposing party identifier")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "Opposing party deleted")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "opposingParty/{opposingPartyId}")] HttpRequest req, string opposingPartyId)
    {
        try
        {
            var officeId = req.GetOfficeId();

            if (string.IsNullOrEmpty(opposingPartyId))
            {
                return new BadRequestObjectResult("opposingPartyId route parameter is required.");
            }

            await _opposingPartyService.Delete(opposingPartyId, officeId);

            await _opposingPartyDeletedPublisher.Publish(new OpposingPartyDeletedMessage
            {
                OfficeId = officeId,
                OpposingPartyId = opposingPartyId,
                OccurredAtUtc = DateTime.UtcNow
            });

            return new NoContentResult();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when deleting opposing party.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting opposing party.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}