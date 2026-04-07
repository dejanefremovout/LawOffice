using System.Net;
using CaseManagement.Api.Extensions;
using CaseManagement.Application.Services;
using CaseManagement.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace CaseManagement.Api.Functions;

/// <summary>
/// HTTP-triggered operations for hearing endpoints.
/// </summary>
public class HearingFunction(ILogger<HearingFunction> logger, IHearingService hearingService)
{
    private readonly ILogger<HearingFunction> _logger = logger;
    private readonly IHearingService _hearingService = hearingService;

    /// <summary>
    /// Gets a hearing by identifier.
    /// </summary>
    [Function("GetHearing")]
    [OpenApiOperation(operationId: "getHearing", tags: ["Hearing"], Summary = "Get a hearing by ID")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiParameter(name: "hearingId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Hearing identifier")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(HearingModel), Description = "The requested hearing")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/plain", bodyType: typeof(string), Description = "Hearing not found")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "hearing/{hearingId}")] HttpRequest req, string hearingId)
    {
        try
        {
            var officeId = req.GetOfficeId();

            if (string.IsNullOrEmpty(hearingId))
            {
                return new BadRequestObjectResult("hearingId route parameter is required.");
            }

            HearingModel? result = await _hearingService.Get(hearingId, officeId);

            if (result == null)
            {
                return new NotFoundObjectResult("Hearing not found.");
            }

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving hearing.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hearing.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Gets all hearings for a specific case.
    /// </summary>
    [Function("GetAllHearings")]
    [OpenApiOperation(operationId: "getAllHearings", tags: ["Hearing"], Summary = "Get all hearings for a case")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiParameter(name: "caseId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Case identifier")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<HearingModel>), Description = "List of hearings")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = "hearing/case/{caseId}")] HttpRequest req, string caseId)
    {
        try
        {
            var officeId = req.GetOfficeId();

            if (string.IsNullOrEmpty(caseId))
            {
                return new BadRequestObjectResult("caseId route parameter is required.");
            }

            IEnumerable<HearingModel> result = await _hearingService.GetAll(caseId, officeId);

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving hearings.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hearings.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Creates a hearing.
    /// </summary>
    [Function("PostHearing")]
    [OpenApiOperation(operationId: "createHearing", tags: ["Hearing"], Summary = "Create a hearing")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(HearingCreateModel), Required = true, Description = "Hearing creation payload")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(HearingModel), Description = "Created hearing")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Function, "post", Route = "hearing")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var hearingModel = JsonConvert.DeserializeObject<HearingCreateModel>(requestBody);

            if (hearingModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            hearingModel = hearingModel with { OfficeId = req.GetOfficeId() };

            HearingModel result = await _hearingService.Create(hearingModel);
            return new CreatedResult($"/hearing/{result.Id}", result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when creating hearing.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hearing.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Updates a hearing.
    /// </summary>
    [Function("PutHearing")]
    [OpenApiOperation(operationId: "updateHearing", tags: ["Hearing"], Summary = "Update a hearing")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(HearingModel), Required = true, Description = "Updated hearing payload")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(HearingModel), Description = "Updated hearing")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> Put([HttpTrigger(AuthorizationLevel.Function, "put", Route = "hearing")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var hearingModel = JsonConvert.DeserializeObject<HearingModel>(requestBody);

            if (hearingModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            hearingModel = hearingModel with { OfficeId = req.GetOfficeId() };

            HearingModel result = await _hearingService.Update(hearingModel);
            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when updating hearing.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hearing.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Deletes a hearing by identifier.
    /// </summary>
    [Function("DeleteHearing")]
    [OpenApiOperation(operationId: "deleteHearing", tags: ["Hearing"], Summary = "Delete a hearing")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiParameter(name: "hearingId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Hearing identifier")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "Hearing deleted")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "hearing/{hearingId}")] HttpRequest req, string hearingId)
    {
        try
        {
            var officeId = req.GetOfficeId();

            if (string.IsNullOrEmpty(hearingId))
            {
                return new BadRequestObjectResult("hearingId route parameter is required.");
            }

            await _hearingService.Delete(hearingId, officeId);

            return new NoContentResult();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when deleting hearing.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hearing.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}