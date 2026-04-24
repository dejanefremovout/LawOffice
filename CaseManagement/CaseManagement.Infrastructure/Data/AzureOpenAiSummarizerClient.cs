using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CaseManagement.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CaseManagement.Infrastructure.Data;

public class AzureOpenAiSummarizerClient(HttpClient httpClient, IConfiguration configuration) : IAiSummarizerClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _endpoint = configuration["AiSettings:Endpoint"] ?? string.Empty;
    private readonly string _apiKey = configuration["AiSettings:ApiKey"] ?? string.Empty;
    private readonly string _deployment = configuration["AiSettings:DeploymentName"] ?? string.Empty;
    private readonly string _apiVersion = configuration["AiSettings:ApiVersion"] ?? "2024-10-21";

    public async Task<string> SummarizeAsync(string content, string summaryDepth, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_endpoint) || string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_deployment))
        {
            throw new InvalidOperationException("AI settings are not configured. Set AiSettings:Endpoint, ApiKey and DeploymentName.");
        }

        string prompt = BuildPrompt(content, summaryDepth);

        var requestPayload = new
        {
            messages = new object[]
            {
                new { role = "system", content = "You summarize legal-office documents. Be concise and factual. Do not provide legal advice." },
                new { role = "user", content = prompt }
            },
            temperature = 0.1,
            max_tokens = 450
        };

        string url = $"{_endpoint.TrimEnd('/')}/openai/deployments/{_deployment}/chat/completions?api-version={_apiVersion}";
        using HttpRequestMessage request = new(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(requestPayload), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("api-key", _apiKey);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"AI request failed with status {(int)response.StatusCode}: {responseBody}");
        }

        using JsonDocument json = JsonDocument.Parse(responseBody);
        string? summary = json.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(summary))
        {
            throw new InvalidOperationException("AI summarizer returned an empty response.");
        }

        return summary;
    }

    private static string BuildPrompt(string content, string summaryDepth)
    {
        string styleInstruction = summaryDepth == "detailed"
            ? "Provide a detailed summary."
            : "Provide a short summary.";

        return $"""
{styleInstruction}
Return plain text with these sections:
Short Summary:
<one concise paragraph>
Key Points:
- bullet
- bullet
Action Items:
- bullet
- bullet

Document:
{content}
""";
    }
}
