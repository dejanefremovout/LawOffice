using CaseManagement.Api.Extensions;
using CaseManagement.Application.Services;
using CaseManagement.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CaseManagement.Api.Functions;

/// <summary>
/// HTTP-triggered operations for document file endpoints.
/// </summary>
public class DocumentFileFunction(ILogger<DocumentFileFunction> logger, IDocumentFileService documentFileService)
{
    private readonly ILogger<DocumentFileFunction> _logger = logger;
    private readonly IDocumentFileService _documentFileService = documentFileService;

    /// <summary>
    /// Gets a document file descriptor by identifier.
    /// </summary>
    [Function("GetDocumentFile")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "documentFile/{documentFileId}")] HttpRequest req, string documentFileId)
    {
        try
        {
            var officeId = req.GetOfficeId();

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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Gets all document file descriptors for a case.
    /// </summary>
    [Function("GetAllDocumentFiles")]
    public async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = "documentFile/case/{caseId}")] HttpRequest req, string caseId)
    {
        try
        {
            var officeId = req.GetOfficeId();

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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Creates a document file descriptor.
    /// </summary>
    [Function("PostDocumentFile")]
    public async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Function, "post", Route = "documentFile")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var documentFileModel = JsonConvert.DeserializeObject<DocumentFileCreateModel>(requestBody);

            if (documentFileModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            documentFileModel = documentFileModel with { OfficeId = req.GetOfficeId() };

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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Updates a document file descriptor.
    /// </summary>
    [Function("PutDocumentFile")]
    public async Task<IActionResult> Put([HttpTrigger(AuthorizationLevel.Function, "put", Route = "documentFile")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var documentFileModel = JsonConvert.DeserializeObject<DocumentFileModel>(requestBody);

            if (documentFileModel == null)
            {
                return new BadRequestObjectResult("Invalid request body.");
            }

            documentFileModel = documentFileModel with { OfficeId = req.GetOfficeId() };

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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    /// <summary>
    /// Deletes a document file descriptor by identifier.
    /// </summary>
    [Function("DeleteDocumentFile")]
    public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "documentFile/{documentFileId}")] HttpRequest req, string documentFileId)
    {
        try
        {
            var officeId = req.GetOfficeId();

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
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}