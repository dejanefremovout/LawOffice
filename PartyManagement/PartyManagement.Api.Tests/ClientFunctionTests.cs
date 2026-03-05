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

public class ClientFunctionTests
{
    private readonly ILogger<ClientFunction> _logger;
    private readonly IClientService _clientService;
    private readonly ClientFunction _function;

    public ClientFunctionTests()
    {
        _logger = Substitute.For<ILogger<ClientFunction>>();
        _clientService = Substitute.For<IClientService>();
        _function = new ClientFunction(_logger, _clientService);
    }

    [Fact]
    public async Task Get_Should_Return_BadRequest_When_ClientId_Is_Missing()
    {
        HttpRequest request = CreateRequest(officeId: "office-1");

        IActionResult actionResult = await _function.Get(request, string.Empty);

        BadRequestObjectResult badRequest = actionResult.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("clientId route parameter is required.");
    }

    [Fact]
    public async Task Get_Should_Return_NotFound_When_Client_Does_Not_Exist()
    {
        HttpRequest request = CreateRequest(officeId: "office-1");
        _clientService.Get("client-1", "office-1").Returns((PartyModel?)null);

        IActionResult actionResult = await _function.Get(request, "client-1");

        NotFoundObjectResult notFound = actionResult.ShouldBeOfType<NotFoundObjectResult>();
        notFound.Value.ShouldBe("Client not found.");
    }

    [Fact]
    public async Task Get_Should_Return_BadRequest_When_Office_Header_Is_Missing()
    {
        HttpRequest request = CreateRequest();

        IActionResult actionResult = await _function.Get(request, "client-1");

        BadRequestObjectResult badRequest = actionResult.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("Office Id header is required.");
    }

    [Fact]
    public async Task GetAll_Should_Return_Ok_With_Clients()
    {
        HttpRequest request = CreateRequest(officeId: "office-1");
        _clientService.GetAll("office-1").Returns(new[]
        {
            new PartyModel { Id = "client-1", OfficeId = "office-1", FirstName = "John", LastName = "Doe" }
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
                    "firstName": "John",
                    "lastName": "Doe",
                    "identificationNumber": "C-1"
                  }
                  """,
            officeId: "office-1");

        _clientService.Create(Arg.Any<PartyCreateModel>())
            .Returns(new PartyModel { Id = "client-1", OfficeId = "office-1", FirstName = "John", LastName = "Doe" });

        IActionResult actionResult = await _function.Post(request);

        CreatedResult created = actionResult.ShouldBeOfType<CreatedResult>();
        created.Location.ShouldBe("/client/office-1/client-1");
        await _clientService.Received(1).Create(Arg.Is<PartyCreateModel>(x => x.OfficeId == "office-1"));
    }

    [Fact]
    public async Task Post_Should_Return_BadRequest_When_Body_Is_Invalid_Json()
    {
        HttpRequest request = CreateRequest(body: "{invalid-json}", officeId: "office-1");

        IActionResult actionResult = await _function.Post(request);

        actionResult.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Post_Should_Return_BadRequest_When_Office_Header_Is_Missing()
    {
        HttpRequest request = CreateRequest(
            body: """
                  {
                    "officeId": "ignored",
                    "firstName": "John",
                    "lastName": "Doe"
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
                    "id": "client-1",
                    "officeId": "ignored",
                    "firstName": "John",
                    "lastName": "Doe"
                  }
                  """,
            officeId: "office-1");

        _clientService.Update(Arg.Any<PartyModel>())
            .Returns(new PartyModel { Id = "client-1", OfficeId = "office-1", FirstName = "John", LastName = "Doe" });

        IActionResult actionResult = await _function.Put(request);

        OkObjectResult okResult = actionResult.ShouldBeOfType<OkObjectResult>();
        PartyModel result = okResult.Value.ShouldBeOfType<PartyModel>();
        result.Id.ShouldBe("client-1");
        await _clientService.Received(1).Update(Arg.Is<PartyModel>(x => x.OfficeId == "office-1"));
    }

    [Fact]
    public async Task Put_Should_Return_BadRequest_When_Body_Is_Invalid_Json()
    {
        HttpRequest request = CreateRequest(body: "{invalid-json}", officeId: "office-1");

        IActionResult actionResult = await _function.Put(request);

        actionResult.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Put_Should_Return_BadRequest_When_Office_Header_Is_Missing()
    {
        HttpRequest request = CreateRequest(
            body: """
                  {
                    "id": "client-1",
                    "officeId": "ignored",
                    "firstName": "John",
                    "lastName": "Doe"
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