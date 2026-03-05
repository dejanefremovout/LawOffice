using Microsoft.AspNetCore.Http;

namespace PartyManagement.Api.Extensions;

/// <summary>
/// HTTP request helper methods for API functions.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Reads and validates the office identifier from request headers.
    /// </summary>
    /// <param name="req">Incoming HTTP request.</param>
    /// <returns>Validated office identifier.</returns>
    /// <exception cref="ArgumentException">Thrown when the required header is missing or empty.</exception>
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
}
