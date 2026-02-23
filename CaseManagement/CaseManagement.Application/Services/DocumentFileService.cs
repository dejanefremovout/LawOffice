using CaseManagement.Domain.Entities;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Domain.ViewModels;

namespace CaseManagement.Application.Services;

public class DocumentFileService(IDocumentFileRepository documentFileRepository,
    IDocumentFileStorageRepository documentFileStorageRepository,
    ICaseRepository caseRepository) : IDocumentFileService
{
    private readonly IDocumentFileRepository _documentFileRepository = documentFileRepository;
    private readonly IDocumentFileStorageRepository _documentFileStorageRepository = documentFileStorageRepository;
    private readonly ICaseRepository _caseRepository = caseRepository;

    public async Task<DocumentFileModel?> Get(string documentFileId, string officeId)
    {
        DocumentFile? documentFileItem = await _documentFileRepository.Get(documentFileId, officeId);
        if (documentFileItem is null)
        {
            return null;
        }

        Uri documentFileUri = await _documentFileStorageRepository.GenerateDownloadUri(documentFileItem.OfficeId, documentFileItem.Id);

        return new DocumentFileModel(documentFileItem, documentFileUri);
    }

    public async Task<IEnumerable<DocumentFileModel>> GetAll(string caseId, string officeId)
    {
        _ = await _caseRepository.Get(caseId, officeId) ?? throw new ArgumentException("Case not found");

        IEnumerable<DocumentFile> documentFiles = await _documentFileRepository.GetAll(caseId, officeId);

        return documentFiles.Select(x => new DocumentFileModel(x));
    }

    public async Task<DocumentFileModel> Create(DocumentFileCreateModel documentFileModel)
    {
        _ = await _caseRepository.Get(documentFileModel.CaseId, documentFileModel.OfficeId) ?? throw new ArgumentException("Case not found");

        DocumentFile documentFileItem = DocumentFile.New(documentFileModel.OfficeId, documentFileModel.CaseId, documentFileModel.Name);

        Uri documentFileUri = await _documentFileStorageRepository.GenerateUploadUri(documentFileItem.OfficeId, documentFileItem.Id);

        documentFileItem = await _documentFileRepository.Add(documentFileItem);

        return new DocumentFileModel(documentFileItem, documentFileUri);
    }

    public async Task<DocumentFileModel> Update(DocumentFileModel documentFileModel)
    {
        DocumentFile? documentFileItem = await _documentFileRepository.Get(documentFileModel.Id, documentFileModel.OfficeId) ?? throw new ArgumentException("Document file not found");

        documentFileItem.SetName(documentFileModel.Name);

        documentFileItem = await _documentFileRepository.Update(documentFileItem);

        return new DocumentFileModel(documentFileItem);
    }

    public async Task Delete(string documentFileId, string officeId)
    {
        _ = await _documentFileRepository.Get(documentFileId, officeId) ?? throw new ArgumentException("Document file not found");

        await _documentFileRepository.Delete(documentFileId, officeId);
    }
}