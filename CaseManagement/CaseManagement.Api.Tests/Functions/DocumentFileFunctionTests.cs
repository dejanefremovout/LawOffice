using CaseManagement.Api.Functions;
using CaseManagement.Api.Tests.TestHelpers;
using CaseManagement.Application.Services;
using CaseManagement.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CaseManagement.Api.Tests.Functions;

public class DocumentFileFunctionTests
{
    private readonly ILogger<DocumentFileFunction> _logger;
    private readonly IDocumentFileService _documentFileService;
    private readonly DocumentFileFunction _sut;

    public DocumentFileFunctionTests()
    {
        _logger = Substitute.For<ILogger<DocumentFileFunction>>();
        _documentFileService = Substitute.For<IDocumentFileService>();
        _sut = new DocumentFileFunction(_logger, _documentFileService);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenDocumentFileIdIsMissing()
    {
        var request = HttpRequestFactory.Create(officeId: "office-1");

        IActionResult result = await _sut.Get(request, string.Empty);

        var badRequest = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("documentFileId route parameter is required.");
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenDocumentFileDoesNotExist()
    {
        var request = HttpRequestFactory.Create(officeId: "office-1");
        _documentFileService.Get("doc-1", "office-1").Returns(Task.FromResult<DocumentFileModel?>(null));

        IActionResult result = await _sut.Get(request, "doc-1");

        var notFound = result.ShouldBeOfType<NotFoundObjectResult>();
        notFound.Value.ShouldBe("Document file not found.");
    }

    [Fact]
    public async Task Put_ReturnsBadRequest_WhenBodyIsInvalid()
    {
        var request = HttpRequestFactory.Create(body: "{invalid-json}", officeId: "office-1");

        IActionResult result = await _sut.Put(request);

        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenRequestIsValid()
    {
        var request = HttpRequestFactory.Create(officeId: "office-1");

        IActionResult result = await _sut.Delete(request, "doc-1");

        result.ShouldBeOfType<NoContentResult>();
        await _documentFileService.Received(1).Delete("doc-1", "office-1");
    }
}