using CaseManagement.Api.Extensions;
using CaseManagement.Application.Services;
using CaseManagement.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CaseManagement.Api.Functions;

public class CaseFunction(ILogger<CaseFunction> logger, ICaseService caseService)
{
    private readonly ILogger<CaseFunction> _logger = logger;
    private readonly ICaseService _caseService = caseService;

    [Function("GetCase")]
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
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("GetAllCases")]
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
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("PostCase")]
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

            req.ValidateOfficeId(caseModel.OfficeId);

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
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("PutCase")]
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

            req.ValidateOfficeId(caseModel.OfficeId);

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
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("DeleteCase")]
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
            return new BadRequestObjectResult(ex.Message);
        }
    }
}