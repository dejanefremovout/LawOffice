using LawOffice.Domain.Entities;
using Newtonsoft.Json;
using System.Net.Mail;

namespace OfficeManagement.Domain.Entities;

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

    public static Lawyer New(Office office, string firstName, string lastName, string email)
    {
        return new(Guid.NewGuid().ToString(), office.Id, true, firstName, lastName, email);
    }

    public void SetName(string firstName, string lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public void SetEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        ValidateEmail(email);

        Email = email.Trim();
    }

    public void SetActive(bool active)
    {
        Active = active;
    }

    public void GenerateNewInvitationCode()
    {
        var code = GenerateRandomCode();
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        InvitationCode = code.Trim();
    }

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