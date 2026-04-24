using CaseManagement.Api.Functions;
using CaseManagement.Api.Tests.TestHelpers;
using CaseManagement.Application.Services;
using CaseManagement.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CaseManagement.Api.Tests.Functions;

public class DocumentSummaryFunctionTests
{
    private readonly ILogger<DocumentSummaryFunction> _logger;
    private readonly IDocumentSummaryService _documentSummaryService;
    private readonly DocumentSummaryFunction _sut;

    public DocumentSummaryFunctionTests()
    {
        _logger = Substitute.For<ILogger<DocumentSummaryFunction>>();
        _documentSummaryService = Substitute.For<IDocumentSummaryService>();
        _sut = new DocumentSummaryFunction(_logger, _documentSummaryService);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenDocumentFileIdMissing()
    {
        HttpRequest request = HttpRequestFactory.Create(officeId: "office-1");

        IActionResult result = await _sut.Post(request, string.Empty);

        var badRequest = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("documentFileId route parameter is required.");
    }

    [Fact]
    public async Task Post_ReturnsTooManyRequests_WhenQuotaExceeded()
    {
        HttpRequest request = HttpRequestFactory.Create(body: "{}", officeId: "office-1");
        _documentSummaryService
            .Summarize("doc-1", "office-1", Arg.Any<DocumentSummaryRequestModel>())
            .Returns(Task.FromException<DocumentSummaryModel>(new AiQuotaExceededException("quota")));

        IActionResult result = await _sut.Post(request, "doc-1");

        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status429TooManyRequests);
        objectResult.Value.ShouldBe("quota");
    }

    [Fact]
    public async Task Post_ReturnsPayloadTooLarge_WhenInputTooLarge()
    {
        HttpRequest request = HttpRequestFactory.Create(body: "{}", officeId: "office-1");
        _documentSummaryService
            .Summarize("doc-1", "office-1", Arg.Any<DocumentSummaryRequestModel>())
            .Returns(Task.FromException<DocumentSummaryModel>(new AiInputTooLargeException("too big")));

        IActionResult result = await _sut.Post(request, "doc-1");

        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status413PayloadTooLarge);
        objectResult.Value.ShouldBe("too big");
    }

    [Fact]
    public async Task Post_ReturnsOk_WhenSummaryGenerated()
    {
        HttpRequest request = HttpRequestFactory.Create(body: "{\"summaryDepth\":\"short\"}", officeId: "office-1");

        _documentSummaryService
            .Summarize("doc-1", "office-1", Arg.Any<DocumentSummaryRequestModel>())
            .Returns(new DocumentSummaryModel
            {
                DocumentFileId = "doc-1",
                SummaryDepth = "short",
                ShortSummary = "Summary",
                KeyPoints = ["k1"],
                ActionItems = ["a1"]
            });

        IActionResult result = await _sut.Post(request, "doc-1");

        var ok = result.ShouldBeOfType<OkObjectResult>();
        var payload = ok.Value.ShouldBeOfType<DocumentSummaryModel>();
        payload.DocumentFileId.ShouldBe("doc-1");
    }
}
