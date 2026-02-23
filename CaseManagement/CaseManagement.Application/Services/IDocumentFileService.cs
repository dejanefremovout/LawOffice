using CaseManagement.Domain.ViewModels;

namespace CaseManagement.Application.Services;

public interface IDocumentFileService
{
    Task<DocumentFileModel?> Get(string documentFileId, string officeId);
    Task<IEnumerable<DocumentFileModel>> GetAll(string caseId, string officeId);
    Task<DocumentFileModel> Create(DocumentFileCreateModel documentFileModel);
    Task<DocumentFileModel> Update(DocumentFileModel documentFileModel);
    Task Delete(string caseId, string officeId);
}
