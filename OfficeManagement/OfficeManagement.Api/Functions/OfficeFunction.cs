using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeManagement.Api.Extensions;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.ViewModels;

namespace OfficeManagement.Api.Functions;

/// <summary>
/// HTTP-triggered operations for office endpoints.
/// </summary>
public class OfficeFunction(ILogger<OfficeFunction> logger, IOfficeService officeService)
{
    private readonly ILogger<OfficeFunction> _logger = logger;
    private readonly IOfficeService _officeService = officeService;

    /// <summary>
    /// Gets office details for the current office context.
    /// </summary>
    [Function("GetOffice")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "office")] HttpRequest req)
    {
        try
        {
            var officeId = req.GetOfficeId();

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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Updates office details for the current office context.
    /// </summary>
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

            officeModel = officeModel with { Id = req.GetOfficeId()};

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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}