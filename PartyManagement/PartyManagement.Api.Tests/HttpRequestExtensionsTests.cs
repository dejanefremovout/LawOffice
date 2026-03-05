using Microsoft.AspNetCore.Http;
using PartyManagement.Api.Extensions;
using Shouldly;

namespace PartyManagement.Api.Tests;

public class HttpRequestExtensionsTests
{
    private readonly DefaultHttpContext _httpContext;

    public HttpRequestExtensionsTests()
    {
        _httpContext = new DefaultHttpContext();
    }

    [Fact]
    public void GetOfficeId_Should_Return_Header_Value()
    {
        _httpContext.Request.Headers["X-Office-Id"] = "office-1";

        string officeId = _httpContext.Request.GetOfficeId();

        officeId.ShouldBe("office-1");
    }

    [Fact]
    public void GetOfficeId_Should_Throw_When_Header_Is_Missing()
    {
        var exception = Should.Throw<ArgumentException>(() => _httpContext.Request.GetOfficeId());

        exception.Message.ShouldBe("Office Id header is required.");
    }

    [Fact]
    public void GetOfficeId_Should_Throw_When_Header_Is_Empty()
    {
        _httpContext.Request.Headers["X-Office-Id"] = " ";

        var exception = Should.Throw<ArgumentException>(() => _httpContext.Request.GetOfficeId());

        exception.Message.ShouldBe("Office Id header cannot be empty.");
    }
}