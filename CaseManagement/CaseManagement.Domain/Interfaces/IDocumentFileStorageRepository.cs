namespace CaseManagement.Domain.Interfaces;

public interface IDocumentFileStorageRepository
{
    Task<Uri> GenerateUploadUri(string officeId, string documentFileId);
    Task<Uri> GenerateDownloadUri(string officeId, string documentFileId);
}