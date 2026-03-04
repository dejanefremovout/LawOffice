using Microsoft.AspNetCore.Http;

namespace PartyManagement.Api.Extensions;

public static class HttpRequestExtensions
{
    public static string GetOfficeId(this HttpRequest req)
    {
        if (!req.Headers.TryGetValue("X-Office-Id", out var officeIdValues))
        {
            throw new ArgumentException("Office Id header is required.");
        }
        var officeId = officeIdValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(officeId))
        {
            throw new ArgumentException("Office Id header cannot be empty.");
        }
        return officeId;
    }

    public static void ValidateOfficeId(this HttpRequest req, string officeId)
    {
        if (string.IsNullOrWhiteSpace(officeId))
        {
            throw new ArgumentException("Office Id header cannot be empty.");
        }
        var headerOfficeId = req.GetOfficeId();
        if (string.IsNullOrWhiteSpace(headerOfficeId))
        {
            throw new ArgumentException("Office Id header cannot be empty.");
        }
        throw new ArgumentException("Office Id is not valid.");
    }
}
