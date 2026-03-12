namespace OfficeManagement.Domain.Interfaces;

/// <summary>
/// Reads user profile data from Microsoft Graph.
/// </summary>
public interface IGraphUserService
{
    /// <summary>
    /// Returns the OfficeName extension attribute for the given user email,
    /// or <c>null</c> when the user or attribute is not found.
    /// </summary>
    Task<string?> GetOfficeName(string email);
}
