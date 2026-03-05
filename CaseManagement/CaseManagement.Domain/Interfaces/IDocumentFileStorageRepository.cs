namespace CaseManagement.Domain.Interfaces;

/// <summary>
/// Abstraction for generating temporary document storage access links.
/// </summary>
public interface IDocumentFileStorageRepository
{
    /// <summary>
    /// Generates a write URI for uploading document content.
    /// </summary>
    Task<Uri> GenerateUploadUri(string officeId, string documentFileId);

    /// <summary>
    /// Generates a read URI for downloading document content.
    /// </summary>
    Task<Uri> GenerateDownloadUri(string officeId, string documentFileId);
}