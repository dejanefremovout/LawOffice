using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PartyManagement.Api.Functions;
using PartyManagement.Application.Services;
using PartyManagement.Domain.ViewModels;
using Shouldly;

namespace PartyManagement.Api.Tests;

public class OpposingPartyFunctionTests
{
    private readonly ILogger<OpposingPartyFunction> _logger;
    private readonly IOpposingPartyService _opposingPartyService;
    private readonly OpposingPartyFunction _function;

    public OpposingPartyFunctionTests()
    {
        _logger = Substitute.For<ILogger<OpposingPartyFunction>>();
        _opposingPartyService = Substitute.For<IOpposingPartyService>();
        _function = new OpposingPartyFunction(_logger, _opposingPartyService);
    }

    [Fact]
    public async Task Get_Should_Return_BadRequest_When_Id_Is_Missing()
    {
        HttpRequest request = CreateRequest(officeId: "office-1");

        IActionResult actionResult = await _function.Get(request, string.Empty);

        BadRequestObjectResult badRequest = actionResult.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("opposingPartyId route parameter is required.");
    }

    [Fact]
    public async Task Get_Should_Return_NotFound_When_OpposingParty_Does_Not_Exist()
    {
        HttpRequest request = CreateRequest(officeId: "office-1");
        _opposingPartyService.Get("op-1", "office-1").Returns((PartyModel?)null);

        IActionResult actionResult = await _function.Get(request, "op-1");

        NotFoundObjectResult notFound = actionResult.ShouldBeOfType<NotFoundObjectResult>();
        notFound.Value.ShouldBe("Opposing party not found.");
    }

    [Fact]
    public async Task Get_Should_Return_BadRequest_When_Office_Header_Is_Missing()
    {
        HttpRequest request = CreateRequest();

        IActionResult actionResult = await _function.Get(request, "op-1");

        BadRequestObjectResult badRequest = actionResult.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("Office Id header is required.");
    }

    [Fact]
    public async Task GetAll_Should_Return_Ok_With_OpposingParties()
    {
        HttpRequest request = CreateRequest(officeId: "office-1");
        _opposingPartyService.GetAll("office-1").Returns(new[]
        {
            new PartyModel { Id = "op-1", OfficeId = "office-1", FirstName = "Alex", LastName = "Smith" }
        });

        IActionResult actionResult = await _function.GetAll(request);

        OkObjectResult okResult = actionResult.ShouldBeOfType<OkObjectResult>();
        IEnumerable<PartyModel> result = okResult.Value.ShouldBeOfType<PartyModel[]>();
        result.Count().ShouldBe(1);
    }

    [Fact]
    public async Task Post_Should_Return_Created_When_Request_Is_Valid()
    {
        HttpRequest request = CreateRequest(
            body: """
                  {
                    "officeId": "ignored",
                    "firstName": "Alex",
                    "lastName": "Smith",
                    "identificationNumber": "OP-1"
                  }
                  """,
            officeId: "office-1");

        _opposingPartyService.Create(Arg.Any<PartyCreateModel>())
            .Returns(new PartyModel { Id = "op-1", OfficeId = "office-1", FirstName = "Alex", LastName = "Smith" });

        IActionResult actionResult = await _function.Post(request);

        CreatedResult created = actionResult.ShouldBeOfType<CreatedResult>();
        created.Location.ShouldBe("/opposingParty/office-1/op-1");
        await _opposingPartyService.Received(1).Create(Arg.Is<PartyCreateModel>(x => x.OfficeId == "office-1"));
    }

    [Fact]
    public async Task Post_Should_Return_BadRequest_When_Body_Is_Invalid_Json()
    {
        HttpRequest request = CreateRequest(body: "{invalid-json}", officeId: "office-1");

        IActionResult actionResult = await _function.Post(request);

           var errorResult = actionResult.ShouldBeOfType<ObjectResult>();
           errorResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
           errorResult.Value.ShouldBe("An unexpected error occurred.");
    }

    [Fact]
    public async Task Post_Should_Return_BadRequest_When_Office_Header_Is_Missing()
    {
        HttpRequest request = CreateRequest(
            body: """
                  {
                    "officeId": "ignored",
                    "firstName": "Alex",
                    "lastName": "Smith"
                  }
                  """);

        IActionResult actionResult = await _function.Post(request);

        BadRequestObjectResult badRequest = actionResult.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("Office Id header is required.");
    }

    [Fact]
    public async Task Put_Should_Return_Ok_When_Request_Is_Valid()
    {
        HttpRequest request = CreateRequest(
            body: """
                  {
                    "id": "op-1",
                    "officeId": "ignored",
                    "firstName": "Alex",
                    "lastName": "Smith"
                  }
                  """,
            officeId: "office-1");

        _opposingPartyService.Update(Arg.Any<PartyModel>())
            .Returns(new PartyModel { Id = "op-1", OfficeId = "office-1", FirstName = "Alex", LastName = "Smith" });

        IActionResult actionResult = await _function.Put(request);

        OkObjectResult okResult = actionResult.ShouldBeOfType<OkObjectResult>();
        PartyModel result = okResult.Value.ShouldBeOfType<PartyModel>();
        result.Id.ShouldBe("op-1");
        await _opposingPartyService.Received(1).Update(Arg.Is<PartyModel>(x => x.OfficeId == "office-1"));
    }

    [Fact]
    public async Task Put_Should_Return_BadRequest_When_Body_Is_Invalid_Json()
    {
        HttpRequest request = CreateRequest(body: "{invalid-json}", officeId: "office-1");

        IActionResult actionResult = await _function.Put(request);

           var errorResult = actionResult.ShouldBeOfType<ObjectResult>();
           errorResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
           errorResult.Value.ShouldBe("An unexpected error occurred.");
    }

    [Fact]
    public async Task Put_Should_Return_BadRequest_When_Office_Header_Is_Missing()
    {
        HttpRequest request = CreateRequest(
            body: """
                  {
                    "id": "op-1",
                    "officeId": "ignored",
                    "firstName": "Alex",
                    "lastName": "Smith"
                  }
                  """);

        IActionResult actionResult = await _function.Put(request);

        BadRequestObjectResult badRequest = actionResult.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("Office Id header is required.");
    }

    private static HttpRequest CreateRequest(string? body = null, string? officeId = null)
    {
        DefaultHttpContext context = new DefaultHttpContext();
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body ?? string.Empty));

        if (officeId is not null)
        {
            context.Request.Headers["X-Office-Id"] = officeId;
        }

        return context.Request;
    }
}