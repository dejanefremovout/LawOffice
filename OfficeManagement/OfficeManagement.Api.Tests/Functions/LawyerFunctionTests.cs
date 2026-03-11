using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OfficeManagement.Api.Functions;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.ViewModels;
using System.Text;

namespace OfficeManagement.Api.Tests.Functions;

public class LawyerFunctionTests
{
    private readonly ILogger<LawyerFunction> _logger;
    private readonly ILawyerService _lawyerService;
    private readonly LawyerFunction _sut;

    public LawyerFunctionTests()
    {
        _logger = Substitute.For<ILogger<LawyerFunction>>();
        _lawyerService = Substitute.For<ILawyerService>();
        _sut = new LawyerFunction(_logger, _lawyerService);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenLawyerIdIsMissing()
    {
        var request = CreateRequest(officeId: "office-1");

        IActionResult result = await _sut.Get(request, string.Empty);

        var badRequest = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("lawyerId route parameter is required.");
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenLawyerDoesNotExist()
    {
        var request = CreateRequest(officeId: "office-1");
        _lawyerService.Get("lawyer-1", "office-1").Returns((LawyerModel?)null);

        IActionResult result = await _sut.Get(request, "lawyer-1");

        var notFound = result.ShouldBeOfType<NotFoundObjectResult>();
        notFound.Value.ShouldBe("Lawyer not found.");
    }

    [Fact]
    public async Task Post_ReturnsInternalServerError_WhenBodyIsInvalid()
    {
        var request = CreateRequest(body: "{invalid-json}", officeId: "office-1");

        IActionResult result = await _sut.Post(request);

           var errorResult = result.ShouldBeOfType<ObjectResult>();
           errorResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
           errorResult.Value.ShouldBe("An unexpected error occurred.");
    }

    [Fact]
    public async Task Post_ReturnsCreated_WhenLawyerIsCreated()
    {
        var requestBody = """
            {
              "firstName": "Jane",
              "lastName": "Doe",
              "email": "jane.doe@example.com",
              "officeId": "ignored"
            }
            """;

        var request = CreateRequest(body: requestBody, officeId: "office-1");
        _lawyerService.Create(Arg.Any<LawyerCreateModel>())
            .Returns(new LawyerModel
            {
                Id = "lawyer-1",
                OfficeId = "office-1",
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com"
            });

        IActionResult result = await _sut.Post(request);

        var created = result.ShouldBeOfType<CreatedResult>();
        created.Location.ShouldBe("/lawyer/lawyer-1");
        await _lawyerService.Received(1).Create(Arg.Is<LawyerCreateModel>(x => x.OfficeId == "office-1"));
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
