using CaseManagement.Domain.ViewModels;

namespace CaseManagement.Application.Services;

public interface IDocumentSummaryService
{
    Task<DocumentSummaryModel> Summarize(string documentFileId, string officeId, DocumentSummaryRequestModel requestModel);
}
