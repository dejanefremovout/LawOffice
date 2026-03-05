using CaseManagement.Domain.ViewModels;

namespace CaseManagement.Application.Services;

/// <summary>
/// Application service for case-related document metadata operations.
/// </summary>
public interface IDocumentFileService
{
    /// <summary>
    /// Gets a document descriptor by identifier.
    /// </summary>
    Task<DocumentFileModel?> Get(string documentFileId, string officeId);

    /// <summary>
    /// Gets all document descriptors for a case.
    /// </summary>
    Task<IEnumerable<DocumentFileModel>> GetAll(string caseId, string officeId);

    /// <summary>
    /// Creates a document descriptor and upload link metadata.
    /// </summary>
    Task<DocumentFileModel> Create(DocumentFileCreateModel documentFileModel);

    /// <summary>
    /// Updates a document descriptor.
    /// </summary>
    Task<DocumentFileModel> Update(DocumentFileModel documentFileModel);

    /// <summary>
    /// Deletes a document descriptor.
    /// </summary>
    Task Delete(string caseId, string officeId);
}
