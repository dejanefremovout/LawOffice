using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CaseManagement.Application.Services;
using CaseManagement.Domain.ViewModels;

namespace CaseManagement.Api.Functions;

public class HearingFunction(ILogger<HearingFunction> logger, IHearingService hearingService)
{
    private readonly ILogger<HearingFunction> _logger = logger;
    private readonly IHearingService _hearingService = hearingService;

    [Function("GetHearing")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "hearing/{officeId}/{hearingId}")] HttpRequest req, string officeId, string hearingId)
    {
        try
        {
            if (string.IsNullOrEmpty(officeId))
            {
                return new BadRequestObjectResult("officeId route parameter is required.");
            }

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
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("GetAllHearings")]
    public async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = "hearing/{officeId}/case/{caseId}")] HttpRequest req, string officeId, string caseId)
    {
        try
        {
            if (string.IsNullOrEmpty(officeId))
            {
                return new BadRequestObjectResult("officeId route parameter is required.");
            }

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
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("PostHearing")]
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
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("PutHearing")]
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
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("DeleteHearing")]
    public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "hearing/{officeId}/{hearingId}")] HttpRequest req, string officeId, string hearingId)
    {
        try
        {
            if (string.IsNullOrEmpty(officeId))
            {
                return new BadRequestObjectResult("officeId route parameter is required.");
            }

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
            return new BadRequestObjectResult(ex.Message);
        }
    }
}