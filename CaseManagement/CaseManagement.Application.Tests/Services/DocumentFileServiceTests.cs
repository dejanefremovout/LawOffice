using CaseManagement.Application.Services;
using CaseManagement.Domain.Entities;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Domain.ViewModels;
using NSubstitute;
using Shouldly;
using CaseEntity = CaseManagement.Domain.Entities.Case;

namespace CaseManagement.Application.Tests.Services;

public class DocumentFileServiceTests
{
    private readonly IDocumentFileRepository _documentFileRepository;
    private readonly IDocumentFileStorageRepository _documentFileStorageRepository;
    private readonly ICaseRepository _caseRepository;
    private readonly DocumentFileService _service;

    public DocumentFileServiceTests()
    {
        _documentFileRepository = Substitute.For<IDocumentFileRepository>();
        _documentFileStorageRepository = Substitute.For<IDocumentFileStorageRepository>();
        _caseRepository = Substitute.For<ICaseRepository>();
        _service = new DocumentFileService(_documentFileRepository, _documentFileStorageRepository, _caseRepository);
    }

    [Fact]
    public async Task Get_WhenDocumentDoesNotExist_ShouldReturnNull()
    {
        _documentFileRepository.Get("doc-1", "office-1").Returns(Task.FromResult<DocumentFile?>(null));

        DocumentFileModel? result = await _service.Get("doc-1", "office-1");

        result.ShouldBeNull();
        await _documentFileStorageRepository.DidNotReceive().GenerateDownloadUri(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Create_WhenCaseExists_ShouldReturnDocumentWithUploadUri()
    {
        _caseRepository.Get("case-1", "office-1")
            .Returns(Task.FromResult<CaseEntity?>(new CaseEntity("case-1", "office-1", ["client-1"], ["opposing-1"], "A-10", null, true, null, null, null, null)));

        _documentFileStorageRepository
            .GenerateUploadUri(Arg.Any<string>(), Arg.Any<string>())
            .Returns(new Uri("https://example.test/upload"));

        _documentFileRepository
            .Add(Arg.Any<DocumentFile>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<DocumentFile>()));

        DocumentFileCreateModel createModel = new()
        {
            CaseId = "case-1",
            OfficeId = "office-1",
            Name = "document.pdf"
        };

        DocumentFileModel result = await _service.Create(createModel);

        result.Name.ShouldBe("document.pdf");
        result.Uri.ShouldNotBeNull();
        result.Uri!.AbsoluteUri.ShouldBe("https://example.test/upload");
    }

    [Fact]
    public async Task Update_WhenDocumentDoesNotExist_ShouldThrowArgumentException()
    {
        DocumentFileModel updateModel = new()
        {
            Id = "doc-1",
            OfficeId = "office-1",
            Name = "updated.pdf"
        };

        _documentFileRepository.Get("doc-1", "office-1").Returns(Task.FromResult<DocumentFile?>(null));

        await Should.ThrowAsync<ArgumentException>(() => _service.Update(updateModel));
        await _documentFileRepository.DidNotReceive().Update(Arg.Any<DocumentFile>());
    }
}