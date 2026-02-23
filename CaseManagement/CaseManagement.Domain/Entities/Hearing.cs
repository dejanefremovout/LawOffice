using LawOffice.Domain.Entities;
using Newtonsoft.Json;

namespace CaseManagement.Domain.Entities;

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

    public static Hearing New(string officeId, string caseId, string? courtroom, string? description, DateTime date)
    {
        return new(Guid.NewGuid().ToString(), officeId, caseId, courtroom, description, date, false);
    }

    public void SetCourtroom(string? courtroom)
    {
        Courtroom = courtroom;
    }

    public void SetDescription(string? description)
    {
        Description = description;
    }

    public void SetDate(DateTime date)
    {
        if (date == default)
        {
            throw new ArgumentException("Date must be a valid date.", nameof(date));
        }
        Date = date;
    }

    public void SetHeld(bool held)
    {
        Held = held;
    }
}