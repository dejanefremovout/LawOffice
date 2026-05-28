using System.Net;
using CaseManagement.Api.Extensions;
using CaseManagement.Application.Services;
using CaseManagement.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace CaseManagement.Api.Functions;

/// <summary>
/// HTTP-triggered operation for AI document summarization.
/// </summary>
public class DocumentSummaryFunction(ILogger<DocumentSummaryFunction> logger, IDocumentSummaryService documentSummaryService)
{
    private readonly ILogger<DocumentSummaryFunction> _logger = logger;
    private readonly IDocumentSummaryService _documentSummaryService = documentSummaryService;

    [Function("PostDocumentSummary")]
    [OpenApiOperation(operationId: "createDocumentSummary", tags: ["DocumentSummary"], Summary = "Create an AI summary for a document")]
    [OpenApiParameter(name: "X-Office-Id", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Tenant office identifier")]
    [OpenApiParameter(name: "documentFileId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Document file identifier")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(DocumentSummaryRequestModel), Required = false, Description = "Summary options")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DocumentSummaryModel), Description = "Generated summary")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.RequestEntityTooLarge, contentType: "text/plain", bodyType: typeof(string), Description = "Input content too large")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.TooManyRequests, contentType: "text/plain", bodyType: typeof(string), Description = "Daily AI quota exceeded")]
    public async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Function, "post", Route = "documentFile/{documentFileId}/summary")] HttpRequest req, string documentFileId)
    {
        try
        {
            string officeId = req.GetOfficeId();

            if (string.IsNullOrWhiteSpace(documentFileId))
            {
                return new BadRequestObjectResult("documentFileId route parameter is required.");
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            DocumentSummaryRequestModel requestModel = string.IsNullOrWhiteSpace(requestBody)
                ? new DocumentSummaryRequestModel()
                : JsonConvert.DeserializeObject<DocumentSummaryRequestModel>(requestBody) ?? new DocumentSummaryRequestModel();

            DocumentSummaryModel summary = await _documentSummaryService.Summarize(documentFileId, officeId, requestModel);
            return new OkObjectResult(summary);
        }
        catch (AiQuotaExceededException ex)
        {
            _logger.LogWarning(ex, "AI daily quota exceeded while creating document summary.");
            return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status429TooManyRequests };
        }
        catch (AiInputTooLargeException ex)
        {
            _logger.LogWarning(ex, "Document content too large for summarization.");
            return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status413PayloadTooLarge };
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided when creating document summary.");
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating document summary.");
            return new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}
