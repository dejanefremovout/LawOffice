using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PartyManagement.Api.Functions;
using PartyManagement.Application.Services;
using PartyManagement.Domain.ViewModels;
using Shouldly;

namespace PartyManagement.Api.Tests;

public class PartyCountFunctionTests
{
    private readonly ILogger<PartyCountFunction> _logger;
    private readonly IClientService _clientService;
    private readonly IOpposingPartyService _opposingPartyService;
    private readonly PartyCountFunction _function;

    public PartyCountFunctionTests()
    {
        _logger = Substitute.For<ILogger<PartyCountFunction>>();
        _clientService = Substitute.For<IClientService>();
        _opposingPartyService = Substitute.For<IOpposingPartyService>();
        _function = new PartyCountFunction(_logger, _clientService, _opposingPartyService);
    }

    [Fact]
    public async Task Get_Should_Return_Ok_With_Party_Counts()
    {
        HttpRequest request = CreateRequest("office-1");
        _clientService.GetCount("office-1").Returns(3);
        _opposingPartyService.GetCount("office-1").Returns(5);

        IActionResult actionResult = await _function.Get(request);

        OkObjectResult okResult = actionResult.ShouldBeOfType<OkObjectResult>();
        PartyCountModel model = okResult.Value.ShouldBeOfType<PartyCountModel>();
        model.ClientsCount.ShouldBe(3);
        model.OpposingPartiesCount.ShouldBe(5);
        await _clientService.Received(1).GetCount("office-1");
        await _opposingPartyService.Received(1).GetCount("office-1");
    }

    [Fact]
    public async Task Get_Should_Return_BadRequest_When_Office_Header_Is_Missing()
    {
        HttpRequest request = CreateRequest(null);

        IActionResult actionResult = await _function.Get(request);

        BadRequestObjectResult badRequest = actionResult.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("Office Id header is required.");
    }

    [Fact]
    public async Task Get_Should_Return_BadRequest_When_Service_Throws_Exception()
    {
        HttpRequest request = CreateRequest("office-1");
        _clientService.GetCount("office-1").Returns(Task.FromException<int>(new InvalidOperationException("failure")));

        IActionResult actionResult = await _function.Get(request);

        BadRequestObjectResult badRequest = actionResult.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("failure");
    }

    private static HttpRequest CreateRequest(string? officeId)
    {
        var context = new DefaultHttpContext();

        if (officeId is not null)
        {
            context.Request.Headers["X-Office-Id"] = officeId;
        }

        return context.Request;
    }
}