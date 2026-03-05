using LawOffice.Domain.Entities;
using Newtonsoft.Json;

namespace CaseManagement.Domain.Entities;

/// <summary>
/// Represents a scheduled hearing associated with a case.
/// </summary>
public class Hearing : Entity
{
    [JsonProperty("officeId")]
    public string OfficeId { get; private set; }

    [JsonProperty("caseId")]
    public string CaseId { get; private set; }

    [JsonProperty("courtroom")]
    public string? Courtroom { get; private set; }

    [JsonProperty("description")]
    public string? Description { get; private set; }

    [JsonProperty("date")]
    public DateTime Date { get; private set; }

    [JsonProperty("held")]
    public bool Held { get; private set; }

    /// <summary>
    /// Initializes a hearing with validated domain values.
    /// </summary>
    public Hearing(string id, string officeId, string caseId, string? courtroom, string? description, DateTime date, bool held)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(caseId);
        if (date == default)
        {
            throw new ArgumentException("Date must be a valid date.", nameof(date));
        }

        Id = id;
        OfficeId = officeId;
        CaseId = caseId;
        Courtroom = courtroom;
        Description = description;
        Date = date;
        Held = held;
    }

    /// <summary>
    /// Factory method for creating a new hearing that is not yet held.
    /// </summary>
    public static Hearing New(string officeId, string caseId, string? courtroom, string? description, DateTime date)
    {
        return new(Guid.NewGuid().ToString(), officeId, caseId, courtroom, description, date, false);
    }

    /// <summary>
    /// Sets the courtroom details.
    /// </summary>
    public void SetCourtroom(string? courtroom)
    {
        Courtroom = courtroom;
    }

    /// <summary>
    /// Sets the hearing description.
    /// </summary>
    public void SetDescription(string? description)
    {
        Description = description;
    }

    /// <summary>
    /// Sets the hearing date.
    /// </summary>
    public void SetDate(DateTime date)
    {
        if (date == default)
        {
            throw new ArgumentException("Date must be a valid date.", nameof(date));
        }
        Date = date;
    }

    /// <summary>
    /// Marks whether the hearing has been held.
    /// </summary>
    public void SetHeld(bool held)
    {
        Held = held;
    }
}