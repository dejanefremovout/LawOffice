using CaseManagement.Domain.Entities;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Domain.ViewModels;

namespace CaseManagement.Application.Services;

public class DocumentSummaryService(
    IDocumentFileRepository documentFileRepository,
    IDocumentFileStorageRepository documentFileStorageRepository,
    IAiSummarizerClient aiSummarizerClient,
    IAiUsageQuotaRepository aiUsageQuotaRepository) : IDocumentSummaryService
{
    private static readonly HashSet<string> SupportedTextExtensions =
        [".txt", ".md", ".json", ".csv", ".xml"];

    private readonly IDocumentFileRepository _documentFileRepository = documentFileRepository;
    private readonly IDocumentFileStorageRepository _documentFileStorageRepository = documentFileStorageRepository;
    private readonly IAiSummarizerClient _aiSummarizerClient = aiSummarizerClient;
    private readonly IAiUsageQuotaRepository _aiUsageQuotaRepository = aiUsageQuotaRepository;

    public async Task<DocumentSummaryModel> Summarize(string documentFileId, string officeId, DocumentSummaryRequestModel requestModel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentFileId);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentNullException.ThrowIfNull(requestModel);

        DocumentFile documentFile = await _documentFileRepository.Get(documentFileId, officeId)
            ?? throw new ArgumentException("Document file not found.");

        string extension = Path.GetExtension(documentFile.Name);
        if (string.IsNullOrWhiteSpace(extension) || !SupportedTextExtensions.Contains(extension.ToLowerInvariant()))
        {
            throw new ArgumentException("Only text-based documents are supported for summarization (.txt, .md, .json, .csv, .xml).");
        }

        int usage = await _aiUsageQuotaRepository.IncrementAndGetUsage(officeId, DateOnly.FromDateTime(DateTime.UtcNow));
        int dailyQuota = ReadPositiveIntSetting("AiSettings:DailyQuotaPerOffice", 15);
        if (usage > dailyQuota)
        {
            throw new AiQuotaExceededException("Daily AI summary quota reached for this office. Please try again tomorrow.");
        }

        string content = await _documentFileStorageRepository.GetTextContent(documentFile.OfficeId, documentFile.Id);
        int maxInputChars = ReadPositiveIntSetting("AiSettings:MaxInputChars", 12_000);
        if (content.Length > maxInputChars)
        {
            throw new AiInputTooLargeException($"Document content exceeds the limit of {maxInputChars} characters.");
        }

        string depth = NormalizeSummaryDepth(requestModel.SummaryDepth);
        string aiResponse = await _aiSummarizerClient.SummarizeAsync(content, depth);

        return new DocumentSummaryModel
        {
            DocumentFileId = documentFile.Id,
            SummaryDepth = depth,
            ShortSummary = aiResponse,
            KeyPoints = ExtractBullets(aiResponse, "Key Points:"),
            ActionItems = ExtractBullets(aiResponse, "Action Items:")
        };
    }

    private static string NormalizeSummaryDepth(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "short";
        }

        string normalized = value.Trim().ToLowerInvariant();
        return normalized switch
        {
            "short" => "short",
            "detailed" => "detailed",
            _ => throw new ArgumentException("summaryDepth must be either 'short' or 'detailed'.")
        };
    }

    private static IEnumerable<string> ExtractBullets(string content, string marker)
    {
        int markerStart = content.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (markerStart < 0)
        {
            return [];
        }

        string section = content[(markerStart + marker.Length)..];
        return section
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(line => line.StartsWith("-", StringComparison.Ordinal))
            .Select(line => line.TrimStart('-', ' '))
            .Take(5)
            .ToArray();
    }

    private static int ReadPositiveIntSetting(string key, int defaultValue)
    {
        string? rawValue = Environment.GetEnvironmentVariable(key);
        if (!int.TryParse(rawValue, out int parsed) || parsed <= 0)
        {
            return defaultValue;
        }

        return parsed;
    }
}
