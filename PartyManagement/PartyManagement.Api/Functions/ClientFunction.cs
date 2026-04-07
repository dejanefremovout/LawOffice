using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using PartyManagement.Api.Extensions;
using PartyManagement.Application.Services;
using PartyManagement.Domain.ViewModels;

namespace PartyManagement.Api.Functions;

/// <summary>
/// HTTP-triggered operations for client endpoints.
/// </summary>
public class ClientFunction(ILogger<ClientFunction> logger, IClientService clientService)
{
    private readonly ILogger<ClientFunction> _logger = logger;
    private readonly IClientService _clientService = clientService;

    /// <summary>
    /// Gets a client by identifier.
    /// </summary>
    [Function("GetClient")]
    [OpenApiOperation(operationId: "getClient", tags: ["Client"], Summary = "Get a client by ID")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiParameter(name: "clientId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Client identifier")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PartyModel), Description = "The requested client")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/plain", bodyType: typeof(string), Description = "Client not found")]
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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Gets all clients for the current office.
    /// </summary>
    [Function("GetAllClients")]
    [OpenApiOperation(operationId: "getAllClients", tags: ["Client"], Summary = "Get all clients")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<PartyModel>), Description = "List of clients")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Creates a client.
    /// </summary>
    [Function("PostClient")]
    [OpenApiOperation(operationId: "createClient", tags: ["Client"], Summary = "Create a client")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(PartyCreateModel), Required = true, Description = "Client creation payload")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(PartyModel), Description = "Created client")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
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

            clientModel = clientModel with { OfficeId = req.GetOfficeId() };

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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Updates a client.
    /// </summary>
    [Function("PutClient")]
    [OpenApiOperation(operationId: "updateClient", tags: ["Client"], Summary = "Update a client")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(PartyModel), Required = true, Description = "Updated client payload")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PartyModel), Description = "Updated client")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
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

            clientModel = clientModel with { OfficeId = req.GetOfficeId() };

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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}