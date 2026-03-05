using Microsoft.AspNetCore.Http;
using System.Text;

namespace CaseManagement.Api.Tests.TestHelpers;

internal static class HttpRequestFactory
{
    public static HttpRequest Create(string? body = null, string? officeId = null)
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