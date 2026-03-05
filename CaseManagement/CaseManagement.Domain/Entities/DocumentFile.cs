using LawOffice.Domain.Entities;
using Newtonsoft.Json;

namespace CaseManagement.Domain.Entities;

/// <summary>
/// Represents document metadata linked to a case.
/// </summary>
public class DocumentFile : Entity
{
    [JsonProperty("officeId")]
    public string OfficeId { get; private set; }

    [JsonProperty("caseId")]
    public string CaseId { get; private set; }

    [JsonProperty("name")]
    public string Name { get; private set; }

    /// <summary>
    /// Initializes a document metadata record with validated values.
    /// </summary>
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

    /// <summary>
    /// Factory method for creating a new document metadata record.
    /// </summary>
    public static DocumentFile New(string officeId, string caseId, string name)
    {
        return new(Guid.NewGuid().ToString(), officeId, caseId, name);
    }

    /// <summary>
    /// Updates the document display name.
    /// </summary>
    public void SetName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }
}