using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using OfficeManagement.Api.Extensions;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.ViewModels;

namespace OfficeManagement.Api.Functions;

/// <summary>
/// HTTP-triggered operations for lawyer endpoints.
/// </summary>
public class LawyerFunction(ILogger<LawyerFunction> logger, ILawyerService lawyerService)
{
    private readonly ILogger<LawyerFunction> _logger = logger;
    private readonly ILawyerService _lawyerService = lawyerService;

    /// <summary>
    /// Gets a lawyer by identifier.
    /// </summary>
    [Function("GetLawyer")]
    [OpenApiOperation(operationId: "getLawyer", tags: ["Lawyer"], Summary = "Get a lawyer by ID")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiParameter(name: "lawyerId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Lawyer identifier")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LawyerModel), Description = "The requested lawyer")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/plain", bodyType: typeof(string), Description = "Lawyer not found")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "lawyer/{lawyerId}")] HttpRequest req, string lawyerId)
    {
        try
        {
            var officeId = req.GetOfficeId();

            if (string.IsNullOrWhiteSpace(lawyerId))
            {
                return new BadRequestObjectResult("lawyerId route parameter is required.");
            }

            LawyerModel? result = await _lawyerService.Get(lawyerId, officeId);

            if (result == null)
            {
                return new NotFoundObjectResult("Lawyer not found.");
            }

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving lawyer.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lawyer.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Gets all lawyers for the current office.
    /// </summary>
    [Function("GetAllLawyers")]
    [OpenApiOperation(operationId: "getAllLawyers", tags: ["Lawyer"], Summary = "Get all lawyers")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<LawyerModel>), Description = "List of lawyers")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = "lawyer")] HttpRequest req)
    {
        try
        {
            var officeId = req.GetOfficeId();

            IEnumerable<LawyerModel> result = await _lawyerService.GetAll(officeId);

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving lawyers.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lawyers.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Creates a lawyer profile.
    /// </summary>
    [Function("PostLawyer")]
    [OpenApiOperation(operationId: "createLawyer", tags: ["Lawyer"], Summary = "Create a lawyer profile")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(LawyerCreateModel), Required = true, Description = "Lawyer creation payload")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(LawyerModel), Description = "Created lawyer")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Function, "post", Route = "lawyer")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var lawyerModel = JsonConvert.DeserializeObject<LawyerCreateModel>(requestBody);

            if (lawyerModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            lawyerModel = lawyerModel with { OfficeId = req.GetOfficeId() };

            LawyerModel result = await _lawyerService.Create(lawyerModel);
            return new CreatedResult($"/lawyer/{result.Id}", result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when creating lawyer.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lawyer.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Updates a lawyer profile.
    /// </summary>
    [Function("PutLawyer")]
    [OpenApiOperation(operationId: "updateLawyer", tags: ["Lawyer"], Summary = "Update a lawyer profile")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(LawyerModel), Required = true, Description = "Updated lawyer payload")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LawyerModel), Description = "Updated lawyer")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> Put([HttpTrigger(AuthorizationLevel.Function, "put", Route = "lawyer")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var lawyerModel = JsonConvert.DeserializeObject<LawyerModel>(requestBody);

            if (lawyerModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            lawyerModel = lawyerModel with { OfficeId = req.GetOfficeId() };

            LawyerModel result = await _lawyerService.Update(lawyerModel);
            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when updating lawyer.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lawyer.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}