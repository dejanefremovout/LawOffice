using CaseManagement.Api.Extensions;
using CaseManagement.Application.Services;
using CaseManagement.Domain.Entities;
using CaseManagement.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CaseManagement.Api.Functions;

public class CaseHearingFunction(ILogger<CaseFunction> logger, ICaseService caseService)
{
    private readonly ILogger<CaseFunction> _logger = logger;
    private readonly ICaseService _caseService = caseService;

    [Function("GetCasesWithHearings")]
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
            return new BadRequestObjectResult(ex.Message);
        }
    }
}