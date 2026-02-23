using CaseManagement.Domain.Entities;

namespace CaseManagement.Domain.ViewModels;

public record DocumentFileModel
{
    public DocumentFileModel()
    {
    }

    public DocumentFileModel(DocumentFile documentFile, Uri? uri = null)
    {
        Id = documentFile.Id;
        OfficeId = documentFile.OfficeId;
        CaseId = documentFile.Id;
        Name = documentFile.Name;
        Uri = uri;
    }

    public string Id { get; init; } = string.Empty;
    public string CaseId { get; init; } = string.Empty;
    public string OfficeId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Uri? Uri { get; init; }
}
