using LawOffice.Domain.Entities;
using Newtonsoft.Json;
using System.Net.Mail;

namespace OfficeManagement.Domain.Entities;

/// <summary>
/// Represents a lawyer account within an office.
/// </summary>
public class Lawyer : Entity
{
    [JsonProperty("officeId")]
    public string OfficeId { get; private set; }

    [JsonProperty("active")]
    public bool Active { get; private set; }

    [JsonProperty("firstName")]
    public string FirstName { get; private set; }

    [JsonProperty("lastName")]
    public string LastName { get; private set; }

    [JsonProperty("email")]
    public string Email { get; private set; }

    [JsonProperty("invitationCode")]
    public string? InvitationCode { get; private set; }

    /// <summary>
    /// Initializes a lawyer with validated identity and contact data.
    /// </summary>
    public Lawyer(string id, string officeId, bool active, string firstName, string lastName, string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        ValidateEmail(email);

        Id = id;
        OfficeId = officeId;
        Active = active;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();
    }

    /// <summary>
    /// Factory method for creating an active lawyer in an office.
    /// </summary>
    public static Lawyer New(Office office, string firstName, string lastName, string email)
    {
        return new(Guid.NewGuid().ToString(), office.Id, true, firstName, lastName, email);
    }

    /// <summary>
    /// Updates first and last name.
    /// </summary>
    public void SetName(string firstName, string lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    /// <summary>
    /// Updates the email address after validation.
    /// </summary>
    public void SetEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        ValidateEmail(email);

        Email = email.Trim();
    }

    /// <summary>
    /// Sets lawyer active status.
    /// </summary>
    public void SetActive(bool active)
    {
        Active = active;
    }

    /// <summary>
    /// Generates and stores a new invitation code.
    /// </summary>
    public void GenerateNewInvitationCode()
    {
        var code = GenerateRandomCode();
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        InvitationCode = code.Trim();
    }

    /// <summary>
    /// Generates an alphanumeric random code.
    /// </summary>
    public static string GenerateRandomCode(int length = 8)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(length, 1);

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var code = new string(Enumerable.Range(0, length)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());

        return code;
    }

    /// <summary>
    /// Validates whether the provided invitation code matches the current one.
    /// </summary>
    public bool ValidateInvitationCode(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        return InvitationCode == code.Trim();
    }

    private static void ValidateEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Email is not a valid address.", nameof(email), ex);
        }
    }
}