using Microsoft.AspNetCore.Http;
using OfficeManagement.Api.Extensions;

namespace OfficeManagement.Api.Tests.Extensions;

public class HttpRequestExtensionsTests
{
    [Fact]
    public void GetOfficeId_Throws_WhenHeaderIsMissing()
    {
        var request = new DefaultHttpContext().Request;

        var exception = Should.Throw<ArgumentException>(() => request.GetOfficeId());

        exception.Message.ShouldBe("Office Id header is required.");
    }

    [Fact]
    public void GetOfficeId_Throws_WhenHeaderIsEmpty()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Office-Id"] = "  ";

        var exception = Should.Throw<ArgumentException>(() => context.Request.GetOfficeId());

        exception.Message.ShouldBe("Office Id header cannot be empty.");
    }

    [Fact]
    public void GetOfficeId_ReturnsHeaderValue_WhenHeaderIsProvided()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Office-Id"] = "office-1";

        string officeId = context.Request.GetOfficeId();

        officeId.ShouldBe("office-1");
    }
}
