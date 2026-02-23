using LawOffice.Domain.Entities;
using Newtonsoft.Json;

namespace CaseManagement.Domain.Entities;

public class Case : Entity
{
    [JsonProperty("officeId")]
    public string OfficeId { get; private set; }

    [JsonProperty("clientIds")]
    public IEnumerable<string> ClientIds { get; private set; }

    [JsonProperty("opposingPartyIds")]
    public IEnumerable<string> OpposingPartyIds { get; private set; }

    [JsonProperty("identificationNumber")]
    public string IdentificationNumber { get; private set; }

    [JsonProperty("description")]
    public string? Description { get; private set; }

    [JsonProperty("active")]
    public bool Active { get; private set; }

    [JsonProperty("competentCourt")]
    public string? CompetentCourt { get; private set; }

    [JsonProperty("city")]
    public string? City { get; private set; }

    [JsonProperty("year")]
    public int? Year { get; private set; }

    [JsonProperty("judge")]
    public string? Judge { get; private set; }


    public Case(string id, string officeId, IEnumerable<string> clientIds, IEnumerable<string> opposingPartyIds, string identificationNumber,
        string? description, bool active, string? competentCourt, string? city, int? year, string? judge)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        if (clientIds == null || !clientIds.Any())
        {
            throw new ArgumentException("At least one client ID must be provided.", nameof(clientIds));
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(identificationNumber);

        Id = id;
        OfficeId = officeId;
        ClientIds = clientIds;
        OpposingPartyIds = opposingPartyIds ?? [];
        IdentificationNumber = identificationNumber;
        Description = description;
        Active = active;
        CompetentCourt = competentCourt;
        City = city;
        Year = year;
        Judge = judge;
    }

    public static Case New(string officeId, IEnumerable<string> clientIds, IEnumerable<string> opposingPartyIds, string identificationNumber,
        string? description, string? competentCourt, string? city, int? year, string? judge)
    {
        return new(Guid.NewGuid().ToString(), officeId, clientIds, opposingPartyIds, identificationNumber, description, true, competentCourt, city, year, judge);
    }

    public void SetClientIds(IEnumerable<string> clientIds)
    {
        if (clientIds == null || !clientIds.Any())
        {
            throw new ArgumentException("At least one client ID must be provided.", nameof(clientIds));
        }
        ClientIds = clientIds;
    }

    public void SetOpposingPartyIds(IEnumerable<string> opposingPartyIds)
    {
        OpposingPartyIds = opposingPartyIds ?? [];
    }

    public void SetIdentificationNumber(string identificationNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identificationNumber);
        IdentificationNumber = identificationNumber.Trim();
    }

    public void UpdateFields(string? description, bool active, string? competentCourt, string? city, int? year, string? judge)
    {
        Description = description?.Trim();
        Active = active;
        CompetentCourt = competentCourt?.Trim();
        City = city?.Trim();
        Year = year;
        Judge = judge?.Trim();
    }
}
