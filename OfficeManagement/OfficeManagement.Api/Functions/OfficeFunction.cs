using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.ViewModels;

namespace OfficeManagement.Api.Functions;

public class OfficeFunction(ILogger<OfficeFunction> logger, IOfficeService officeService)
{
    private readonly ILogger<OfficeFunction> _logger = logger;
    private readonly IOfficeService _officeService = officeService;

    [Function("GetOffice")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "office/{officeId}")] HttpRequest req, string officeId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(officeId))
            {
                return new BadRequestObjectResult("officeId route parameter is required.");
            }

            OfficeModel? result = await _officeService.Get(officeId);

            if (result == null)
            {
                return new NotFoundObjectResult("Office not found.");
            }

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving office.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving office.");
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("PostOffice")]
    public async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Function, "post", Route = "office")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var officeModel = JsonConvert.DeserializeObject<OfficeCreateModel>(requestBody);

            if (officeModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            OfficeModel result = await _officeService.Create(officeModel);
            return new CreatedResult($"/office/{result.Id}", result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when creating office.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating office.");
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("PutOffice")]
    public async Task<IActionResult> Put([HttpTrigger(AuthorizationLevel.Function, "put", Route = "office")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var officeModel = JsonConvert.DeserializeObject<OfficeModel>(requestBody);

            if (officeModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            OfficeModel result = await _officeService.Update(officeModel);
            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when updating office.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating office.");
            return new BadRequestObjectResult(ex.Message);
        }
    }
}