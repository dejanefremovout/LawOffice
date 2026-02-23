using LawOffice.Domain.Entities;
using Newtonsoft.Json;

namespace CaseManagement.Domain.Entities;

public class DocumentFile : Entity
{
    [JsonProperty("officeId")]
    public string OfficeId { get; private set; }

    [JsonProperty("caseId")]
    public string CaseId { get; private set; }

    [JsonProperty("name")]
    public string Name { get; private set; }

    public DocumentFile(string id, string officeId, string caseId, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(caseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        OfficeId = officeId;
        CaseId = caseId;
        Name = name;
    }

    public static DocumentFile New(string officeId, string caseId, string name)
    {
        return new(Guid.NewGuid().ToString(), officeId, caseId, name);
    }

    public void SetName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }
}