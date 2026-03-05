using CaseManagement.Domain.Entities;
using LawOffice.Domain.Interfaces;

namespace CaseManagement.Domain.Interfaces;

/// <summary>
/// Repository contract for document file metadata persistence.
/// </summary>
public interface IDocumentFileRepository : IRepository<DocumentFile>
{
    /// <summary>
    /// Persists a new document file metadata record.
    /// </summary>
    Task<DocumentFile> Add(DocumentFile documentFile);

    /// <summary>
    /// Persists updates to an existing document file metadata record.
    /// </summary>
    Task<DocumentFile> Update(DocumentFile documentFile);

    /// <summary>
    /// Gets a document file metadata record by identifier.
    /// </summary>
    Task<DocumentFile?> Get(string documentFileId, string officeId);

    /// <summary>
    /// Gets all document file metadata records for a case.
    /// </summary>
    Task<IEnumerable<DocumentFile>> GetAll(string caseId, string officeId);

    /// <summary>
    /// Deletes a document file metadata record.
    /// </summary>
    Task Delete(string documentFileId, string officeId);
}