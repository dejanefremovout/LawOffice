using System.Net;
using CaseManagement.Api.Extensions;
using CaseManagement.Application.Services;
using CaseManagement.Domain.Entities;
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
/// HTTP-triggered operations for case management endpoints.
/// </summary>
public class CaseFunction(ILogger<CaseFunction> logger, ICaseService caseService)
{
    private readonly ILogger<CaseFunction> _logger = logger;
    private readonly ICaseService _caseService = caseService;

    /// <summary>
    /// Gets a case by identifier.
    /// </summary>
    [Function("GetCase")]
    [OpenApiOperation(operationId: "getCase", tags: ["Case"], Summary = "Get a case by ID")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiParameter(name: "caseId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Case identifier")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CaseModel), Description = "The requested case")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/plain", bodyType: typeof(string), Description = "Case not found")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "case/{caseId}")] HttpRequest req, string caseId)
    {
        try
        {
            var officeId = req.GetOfficeId();

            if (string.IsNullOrEmpty(caseId))
            {
                return new BadRequestObjectResult("caseId route parameter is required.");
            }

            CaseModel? result = await _caseService.Get(caseId, officeId);

            if (result == null)
            {
                return new NotFoundObjectResult("Case not found.");
            }

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving case.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving case.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Gets all cases for the current office.
    /// </summary>
    [Function("GetAllCases")]
    [OpenApiOperation(operationId: "getAllCases", tags: ["Case"], Summary = "Get all cases")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<CaseModel>), Description = "List of cases")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = "case")] HttpRequest req)
    {
        try
        {
            var officeId = req.GetOfficeId();

            IEnumerable<CaseModel> result = await _caseService.GetAll(officeId);

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving cases.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cases.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Creates a new case.
    /// </summary>
    [Function("PostCase")]
    [OpenApiOperation(operationId: "createCase", tags: ["Case"], Summary = "Create a new case")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CaseCreateModel), Required = true, Description = "Case creation payload")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(CaseModel), Description = "Created case")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Function, "post", Route = "case")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var caseModel = JsonConvert.DeserializeObject<CaseCreateModel>(requestBody);

            if (caseModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            caseModel = caseModel with { OfficeId = req.GetOfficeId() };

            CaseModel result = await _caseService.Create(caseModel);
            return new CreatedResult($"/case/{result.Id}", result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when creating case.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating case.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Updates an existing case.
    /// </summary>
    [Function("PutCase")]
    [OpenApiOperation(operationId: "updateCase", tags: ["Case"], Summary = "Update an existing case")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CaseModel), Required = true, Description = "Updated case payload")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CaseModel), Description = "Updated case")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> Put([HttpTrigger(AuthorizationLevel.Function, "put", Route = "case")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var caseModel = JsonConvert.DeserializeObject<CaseModel>(requestBody);

            if (caseModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            caseModel = caseModel with { OfficeId = req.GetOfficeId() };

            CaseModel result = await _caseService.Update(caseModel);
            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when updating case.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating case.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Deletes a case by identifier.
    /// </summary>
    [Function("DeleteCase")]
    [OpenApiOperation(operationId: "deleteCase", tags: ["Case"], Summary = "Delete a case")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiParameter(name: "caseId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Case identifier")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "Case deleted")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "case/{caseId}")] HttpRequest req, string caseId)
    {
        try
        {
            var officeId = req.GetOfficeId();

            if (string.IsNullOrEmpty(caseId))
            {
                return new BadRequestObjectResult("caseId route parameter is required.");
            }

            await _caseService.Delete(caseId, officeId);

            return new NoContentResult();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when deleting case.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting case.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Returns case count aggregates for the current office.
    /// </summary>
    [Function("GetCount")]
    [OpenApiOperation(operationId: "getCaseCount", tags: ["Case"], Summary = "Get case count aggregates")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CaseCountModel), Description = "Case count aggregates")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> GetCount([HttpTrigger(AuthorizationLevel.Function, "get", Route = "cases/count")] HttpRequest req)
    {
        try
        {
            var officeId = req.GetOfficeId();

            var result = await _caseService.GetCount(officeId);

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving case count.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving case count.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Gets the most recent active cases.
    /// </summary>
    [Function("GetLastCases")]
    [OpenApiOperation(operationId: "getLastCases", tags: ["Case"], Summary = "Get the most recent active cases")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiParameter(name: "count", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "Number of recent cases to return")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<CaseModel>), Description = "List of recent active cases")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> GetLastCases([HttpTrigger(AuthorizationLevel.Function, "get", Route = "cases/last/{count}")] HttpRequest req, int count)
    {
        try
        {
            if (count <= 0)
            {
                return new BadRequestObjectResult("Case count route parameter is required.");
            }

            var officeId = req.GetOfficeId();

            IEnumerable<CaseModel> result = await _caseService.GetLastActiveCases(officeId, count);

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving cases.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cases.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}