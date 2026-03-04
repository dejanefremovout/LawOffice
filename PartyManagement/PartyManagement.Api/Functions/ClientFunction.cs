using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PartyManagement.Api.Extensions;
using PartyManagement.Application.Services;
using PartyManagement.Domain.ViewModels;

namespace PartyManagement.Api.Functions;

public class ClientFunction(ILogger<ClientFunction> logger, IClientService clientService)
{
    private readonly ILogger<ClientFunction> _logger = logger;
    private readonly IClientService _clientService = clientService;

    [Function("GetClient")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "client/{clientId}")] HttpRequest req, string clientId)
    {
        try
        {
            var officeId = req.GetOfficeId();

            if (string.IsNullOrEmpty(clientId))
            {
                return new BadRequestObjectResult("clientId route parameter is required.");
            }

            PartyModel? result = await _clientService.Get(clientId, officeId);

            return result == null ? new NotFoundObjectResult("Client not found.") : new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving client.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving client.");
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("GetAllClients")]
    public async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = "client")] HttpRequest req)
    {
        try
        {
            var officeId = req.GetOfficeId();

            IEnumerable<PartyModel> result = await _clientService.GetAll(officeId);

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving clients.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving clients.");
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("PostClient")]
    public async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Function, "post", Route = "client")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            PartyCreateModel? clientModel = JsonConvert.DeserializeObject<PartyCreateModel>(requestBody);

            if (clientModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            req.ValidateOfficeId(clientModel.OfficeId);

            PartyModel result = await _clientService.Create(clientModel);
            return new CreatedResult($"/client/{result.OfficeId}/{result.Id}", result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when creating client.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client.");
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("PutClient")]
    public async Task<IActionResult> Put([HttpTrigger(AuthorizationLevel.Function, "put", Route = "client")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            PartyModel? clientModel = JsonConvert.DeserializeObject<PartyModel>(requestBody);

            if (clientModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            req.ValidateOfficeId(clientModel.OfficeId);

            PartyModel result = await _clientService.Update(clientModel);
            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when updating client.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating client.");
            return new BadRequestObjectResult(ex.Message);
        }
    }
}