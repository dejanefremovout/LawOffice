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
/// HTTP-triggered operations that combine case and hearing data.
/// </summary>
public class CaseHearingFunction(ILogger<CaseFunction> logger, ICaseService caseService)
{
    private readonly ILogger<CaseFunction> _logger = logger;
    private readonly ICaseService _caseService = caseService;

    /// <summary>
    /// Gets the next upcoming hearings projected with case identifiers.
    /// </summary>
    [Function("GetCasesWithHearings")]
    [OpenApiOperation(operationId: "getCasesWithHearings", tags: ["CaseHearing"], Summary = "Get upcoming hearings with case info")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiParameter(name: "count", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "Number of upcoming hearings to return")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<CaseHearingModel>), Description = "List of case hearings")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<IActionResult> GetCasesWithHearings([HttpTrigger(AuthorizationLevel.Function, "get", Route = "cases/hearings/{count}")] HttpRequest req, int count)
    {
        try
        {
            if (count <= 0)
            {
                return new BadRequestObjectResult("Case count route parameter is required.");
            }

            var officeId = req.GetOfficeId();

            IEnumerable<CaseHearingModel> result = await _caseService.GetCasesWithUpcomingHearings(officeId, count);

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