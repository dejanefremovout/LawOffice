using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CaseManagement.Application.Services;
using CaseManagement.Domain.ViewModels;

namespace CaseManagement.Api.Functions;

public class DocumentFileFunction(ILogger<DocumentFileFunction> logger, IDocumentFileService documentFileService)
{
    private readonly ILogger<DocumentFileFunction> _logger = logger;
    private readonly IDocumentFileService _documentFileService = documentFileService;

    [Function("GetDocumentFile")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documentFile/{officeId}/{documentFileId}")] HttpRequest req, string officeId, string documentFileId)
    {
        try
        {
            if (string.IsNullOrEmpty(officeId))
            {
                return new BadRequestObjectResult("officeId route parameter is required.");
            }

            if (string.IsNullOrEmpty(documentFileId))
            {
                return new BadRequestObjectResult("documentFileId route parameter is required.");
            }

            DocumentFileModel? result = await _documentFileService.Get(documentFileId, officeId);

            if (result == null)
            {
                return new NotFoundObjectResult("Document file not found.");
            }

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving document file.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving document file.");
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("GetAllDocumentFiles")]
    public async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documentFile/{officeId}/case/{caseId}")] HttpRequest req, string officeId, string caseId)
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

            IEnumerable<DocumentFileModel> result = await _documentFileService.GetAll(caseId, officeId);

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when retrieving document files.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving document files.");
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("PostDocumentFile")]
    public async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "documentFile")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var documentFileModel = JsonConvert.DeserializeObject<DocumentFileCreateModel>(requestBody);

            if (documentFileModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            DocumentFileModel result = await _documentFileService.Create(documentFileModel);
            return new CreatedResult($"/documentFile/{result.Id}", result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when creating document file.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating document file.");
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("PutDocumentFile")]
    public async Task<IActionResult> Put([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "documentFile")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var documentFileModel = JsonConvert.DeserializeObject<DocumentFileModel>(requestBody);

            if (documentFileModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            DocumentFileModel result = await _documentFileService.Update(documentFileModel);
            return new OkObjectResult(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when updating document file.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating document file.");
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("DeleteDocumentFile")]
    public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "documentFile/{officeId}/{documentFileId}")] HttpRequest req, string officeId, string documentFileId)
    {
        try
        {
            if (string.IsNullOrEmpty(officeId))
            {
                return new BadRequestObjectResult("officeId route parameter is required.");
            }

            if (string.IsNullOrEmpty(documentFileId))
            {
                return new BadRequestObjectResult("documentFileId route parameter is required.");
            }

            await _documentFileService.Delete(documentFileId, officeId);

            return new NoContentResult();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when deleting document file.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document file.");
            return new BadRequestObjectResult(ex.Message);
        }
    }
}