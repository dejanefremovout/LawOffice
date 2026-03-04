using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeManagement.Api.Extensions;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.ViewModels;

namespace OfficeManagement.Api.Functions;

public class LawyerFunction(ILogger<LawyerFunction> logger, ILawyerService lawyerService)
{
    private readonly ILogger<LawyerFunction> _logger = logger;
    private readonly ILawyerService _lawyerService = lawyerService;

    [Function("GetLawyer")]
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
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("GetAllLawyers")]
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
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("PostLawyer")]
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

            req.ValidateOfficeId(lawyerModel.OfficeId);

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
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("PutLawyer")]
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

            req.ValidateOfficeId(lawyerModel.OfficeId);

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
            return new BadRequestObjectResult(ex.Message);
        }
    }
}