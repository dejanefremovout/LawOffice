using CaseManagement.Application.Services;
using CaseManagement.Domain.Entities;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Domain.ViewModels;
using NSubstitute;
using Shouldly;

namespace CaseManagement.Application.Tests.Services;

public class DocumentSummaryServiceTests
{
    private readonly IDocumentFileRepository _documentFileRepository;
    private readonly IDocumentFileStorageRepository _documentFileStorageRepository;
    private readonly IAiSummarizerClient _aiSummarizerClient;
    private readonly IAiUsageQuotaRepository _aiUsageQuotaRepository;
    private readonly DocumentSummaryService _service;

    public DocumentSummaryServiceTests()
    {
        _documentFileRepository = Substitute.For<IDocumentFileRepository>();
        _documentFileStorageRepository = Substitute.For<IDocumentFileStorageRepository>();
        _aiSummarizerClient = Substitute.For<IAiSummarizerClient>();
        _aiUsageQuotaRepository = Substitute.For<IAiUsageQuotaRepository>();
        _service = new DocumentSummaryService(_documentFileRepository, _documentFileStorageRepository, _aiSummarizerClient, _aiUsageQuotaRepository);
    }

    [Fact]
    public async Task Summarize_WhenDocumentNotFound_ShouldThrowArgumentException()
    {
        _documentFileRepository.Get("doc-1", "office-1").Returns(Task.FromResult<DocumentFile?>(null));

        await Should.ThrowAsync<ArgumentException>(() => _service.Summarize("doc-1", "office-1", new DocumentSummaryRequestModel()));
    }

    [Fact]
    public async Task Summarize_WhenQuotaExceeded_ShouldThrowAiQuotaExceededException()
    {
        _documentFileRepository.Get("doc-1", "office-1")
            .Returns(Task.FromResult<DocumentFile?>(new DocumentFile("doc-1", "office-1", "case-1", "notes.txt")));
        _aiUsageQuotaRepository.IncrementAndGetUsage("office-1", Arg.Any<DateOnly>()).Returns(16);

        await Should.ThrowAsync<AiQuotaExceededException>(() => _service.Summarize("doc-1", "office-1", new DocumentSummaryRequestModel()));
        await _documentFileStorageRepository.DidNotReceive().GetTextContent(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Summarize_WhenContentTooLarge_ShouldThrowAiInputTooLargeException()
    {
        _documentFileRepository.Get("doc-1", "office-1")
            .Returns(Task.FromResult<DocumentFile?>(new DocumentFile("doc-1", "office-1", "case-1", "notes.txt")));
        _aiUsageQuotaRepository.IncrementAndGetUsage("office-1", Arg.Any<DateOnly>()).Returns(1);
        _documentFileStorageRepository.GetTextContent("office-1", "doc-1")
            .Returns(new string('x', 12001));

        await Should.ThrowAsync<AiInputTooLargeException>(() => _service.Summarize("doc-1", "office-1", new DocumentSummaryRequestModel()));
        await _aiSummarizerClient.DidNotReceive().SummarizeAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Summarize_WhenValidRequest_ShouldReturnSummary()
    {
        _documentFileRepository.Get("doc-1", "office-1")
            .Returns(Task.FromResult<DocumentFile?>(new DocumentFile("doc-1", "office-1", "case-1", "notes.txt")));
        _aiUsageQuotaRepository.IncrementAndGetUsage("office-1", Arg.Any<DateOnly>()).Returns(1);
        _documentFileStorageRepository.GetTextContent("office-1", "doc-1").Returns("Content");
        _aiSummarizerClient.SummarizeAsync("Content", "detailed", Arg.Any<CancellationToken>())
            .Returns("Short Summary:\nSummary\nKey Points:\n- key1\nAction Items:\n- action1");

        DocumentSummaryModel result = await _service.Summarize("doc-1", "office-1", new DocumentSummaryRequestModel { SummaryDepth = "detailed" });

        result.DocumentFileId.ShouldBe("doc-1");
        result.SummaryDepth.ShouldBe("detailed");
        result.ShortSummary.ShouldContain("Short Summary:");
    }
}
