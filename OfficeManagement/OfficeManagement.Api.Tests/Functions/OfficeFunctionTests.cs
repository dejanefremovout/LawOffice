using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OfficeManagement.Api.Functions;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.ViewModels;
using System.Text;

namespace OfficeManagement.Api.Tests.Functions;

public class OfficeFunctionTests
{
    private readonly ILogger<OfficeFunction> _logger;
    private readonly IOfficeService _officeService;
    private readonly OfficeFunction _sut;

    public OfficeFunctionTests()
    {
        _logger = Substitute.For<ILogger<OfficeFunction>>();
        _officeService = Substitute.For<IOfficeService>();
        _sut = new OfficeFunction(_logger, _officeService);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenOfficeHeaderIsMissing()
    {
        var request = CreateRequest();

        IActionResult result = await _sut.Get(request);

        var badRequest = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("Office Id header is required.");
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenOfficeDoesNotExist()
    {
        var request = CreateRequest(officeId: "office-1");
        _officeService.Get("office-1").Returns((OfficeModel?)null);

        IActionResult result = await _sut.Get(request);

        var notFound = result.ShouldBeOfType<NotFoundObjectResult>();
        notFound.Value.ShouldBe("Office not found.");
    }

    [Fact]
    public async Task Get_ReturnsOk_WhenOfficeExists()
    {
        var request = CreateRequest(officeId: "office-1");
        _officeService.Get("office-1").Returns(new OfficeModel { Id = "office-1", Name = "HQ" });

        IActionResult result = await _sut.Get(request);

        var ok = result.ShouldBeOfType<OkObjectResult>();
        var office = ok.Value.ShouldBeOfType<OfficeModel>();
        office.Id.ShouldBe("office-1");
    }

    [Fact]
    public async Task Put_ReturnsInternalServerError_WhenBodyIsInvalid()
    {
        var request = CreateRequest(body: "{invalid-json}", officeId: "office-1");

        IActionResult result = await _sut.Put(request);

           var errorResult = result.ShouldBeOfType<ObjectResult>();
           errorResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
           errorResult.Value.ShouldBe("An unexpected error occurred.");
    }

    private static HttpRequest CreateRequest(string? body = null, string? officeId = null)
    {
        var context = new DefaultHttpContext();

        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body ?? string.Empty));

        if (officeId is not null)
        {
            context.Request.Headers["X-Office-Id"] = officeId;
        }

        return context.Request;
    }
}
