using CaseManagement.Api.Functions;
using CaseManagement.Api.Tests.TestHelpers;
using CaseManagement.Application.Services;
using CaseManagement.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CaseManagement.Api.Tests.Functions;

public class HearingFunctionTests
{
    private readonly ILogger<HearingFunction> _logger;
    private readonly IHearingService _hearingService;
    private readonly HearingFunction _sut;

    public HearingFunctionTests()
    {
        _logger = Substitute.For<ILogger<HearingFunction>>();
        _hearingService = Substitute.For<IHearingService>();
        _sut = new HearingFunction(_logger, _hearingService);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenHearingIdIsMissing()
    {
        var request = HttpRequestFactory.Create(officeId: "office-1");

        IActionResult result = await _sut.Get(request, string.Empty);

        var badRequest = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("hearingId route parameter is required.");
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenHearingDoesNotExist()
    {
        var request = HttpRequestFactory.Create(officeId: "office-1");
        _hearingService.Get("hearing-1", "office-1").Returns(Task.FromResult<HearingModel?>(null));

        IActionResult result = await _sut.Get(request, "hearing-1");

        var notFound = result.ShouldBeOfType<NotFoundObjectResult>();
        notFound.Value.ShouldBe("Hearing not found.");
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenBodyIsInvalid()
    {
        var request = HttpRequestFactory.Create(body: "{invalid-json}", officeId: "office-1");

        IActionResult result = await _sut.Post(request);

        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenRequestIsValid()
    {
        var request = HttpRequestFactory.Create(officeId: "office-1");

        IActionResult result = await _sut.Delete(request, "hearing-1");

        result.ShouldBeOfType<NoContentResult>();
        await _hearingService.Received(1).Delete("hearing-1", "office-1");
    }
}