using Newtonsoft.Json;

namespace CaseManagement.Domain.Entities;

/// <summary>
/// Daily AI usage counter per office.
/// </summary>
public class AiUsageQuota
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("officeId")]
    public string OfficeId { get; set; } = string.Empty;

    [JsonProperty("usageDate")]
    public string UsageDate { get; set; } = string.Empty;

    [JsonProperty("requestCount")]
    public int RequestCount { get; set; }
}
