using CaseManagement.Api.Functions;
using CaseManagement.Api.Tests.TestHelpers;
using CaseManagement.Application.Services;
using CaseManagement.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CaseManagement.Api.Tests.Functions;

public class CaseHearingFunctionTests
{
    private readonly ILogger<CaseFunction> _logger;
    private readonly ICaseService _caseService;
    private readonly CaseHearingFunction _sut;

    public CaseHearingFunctionTests()
    {
        _logger = Substitute.For<ILogger<CaseFunction>>();
        _caseService = Substitute.For<ICaseService>();
        _sut = new CaseHearingFunction(_logger, _caseService);
    }

    [Fact]
    public async Task GetCasesWithHearings_ReturnsBadRequest_WhenCountIsNotPositive()
    {
        var request = HttpRequestFactory.Create(officeId: "office-1");

        IActionResult result = await _sut.GetCasesWithHearings(request, 0);

        var badRequest = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("Case count route parameter is required.");
    }

    [Fact]
    public async Task GetCasesWithHearings_ReturnsOk_WhenRequestIsValid()
    {
        var request = HttpRequestFactory.Create(officeId: "office-1");
        _caseService.GetCasesWithUpcomingHearings("office-1", 2)
            .Returns(Task.FromResult<IEnumerable<CaseHearingModel>>([new CaseHearingModel { Id = "case-1", IdentificationNumber = "A-1", Date = DateTime.UtcNow } ]));

        IActionResult result = await _sut.GetCasesWithHearings(request, 2);

        var ok = result.ShouldBeOfType<OkObjectResult>();
        ok.Value.ShouldNotBeNull();
        var payload = ok.Value.ShouldBeAssignableTo<IEnumerable<CaseHearingModel>>();
        payload.Count().ShouldBe(1);
    }
}