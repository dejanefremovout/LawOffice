using CaseManagement.Api.Functions;
using CaseManagement.Api.Tests.TestHelpers;
using CaseManagement.Application.Services;
using CaseManagement.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Microsoft.AspNetCore.Http;

namespace CaseManagement.Api.Tests.Functions;

public class CaseFunctionTests
{
    private readonly ILogger<CaseFunction> _logger;
    private readonly ICaseService _caseService;
    private readonly CaseFunction _sut;

    public CaseFunctionTests()
    {
        _logger = Substitute.For<ILogger<CaseFunction>>();
        _caseService = Substitute.For<ICaseService>();
        _sut = new CaseFunction(_logger, _caseService);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenOfficeHeaderIsMissing()
    {
        var request = HttpRequestFactory.Create();

        IActionResult result = await _sut.Get(request, "case-1");

        var badRequest = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("Office Id header is required.");
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenCaseDoesNotExist()
    {
        var request = HttpRequestFactory.Create(officeId: "office-1");
        _caseService.Get("case-1", "office-1").Returns(Task.FromResult<CaseModel?>(null));

        IActionResult result = await _sut.Get(request, "case-1");

        var notFound = result.ShouldBeOfType<NotFoundObjectResult>();
        notFound.Value.ShouldBe("Case not found.");
    }

    [Fact]
    public async Task Post_ReturnsInternalServerError_WhenBodyIsInvalid()
    {
        var request = HttpRequestFactory.Create(body: "{invalid-json}", officeId: "office-1");

        IActionResult result = await _sut.Post(request);

           var errorResult = result.ShouldBeOfType<ObjectResult>();
           errorResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
           errorResult.Value.ShouldBe("An unexpected error occurred.");
    }

    [Fact]
    public async Task Post_ReturnsCreated_WhenBodyIsValid()
    {
        var request = HttpRequestFactory.Create(
            body: "{\"clientIds\":[\"client-1\"],\"opposingPartyIds\":[],\"identificationNumber\":\"A-1\"}",
            officeId: "office-1");

        _caseService.Create(Arg.Any<CaseCreateModel>())
            .Returns(Task.FromResult(new CaseModel { Id = "case-1", OfficeId = "office-1" }));

        IActionResult result = await _sut.Post(request);

        var created = result.ShouldBeOfType<CreatedResult>();
        created.Location.ShouldBe("/case/case-1");
        await _caseService.Received(1).Create(Arg.Is<CaseCreateModel>(x => x.OfficeId == "office-1"));
    }

    [Fact]
    public async Task GetLastCases_ReturnsBadRequest_WhenCountIsNotPositive()
    {
        var request = HttpRequestFactory.Create(officeId: "office-1");

        IActionResult result = await _sut.GetLastCases(request, 0);

        var badRequest = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("Case count route parameter is required.");
    }
}