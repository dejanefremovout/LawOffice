using CaseManagement.Domain.Entities;
using LawOffice.Domain.Interfaces;

namespace CaseManagement.Domain.Interfaces;

public interface IDocumentFileRepository : IRepository<DocumentFile>
{
    Task<DocumentFile> Add(DocumentFile documentFile);

    Task<DocumentFile> Update(DocumentFile documentFile);

    Task<DocumentFile?> Get(string documentFileId, string officeId);

    Task<IEnumerable<DocumentFile>> GetAll(string caseId, string officeId);

    Task Delete(string documentFileId, string officeId);
}