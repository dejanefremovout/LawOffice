using CaseManagement.Domain.Entities;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Infrastructure.Data;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace CaseManagement.Infrastructure.Repositories;

public class AiUsageQuotaRepository(ICosmosService cosmosService) : IAiUsageQuotaRepository
{
    private const string AiUsageContainerId = "aiusagequotas";
    private readonly Container _container = cosmosService.GetContainer(AiUsageContainerId);

    public async Task<int> IncrementAndGetUsage(string officeId, DateOnly usageDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        string dateKey = usageDate.ToString("yyyy-MM-dd");
        string id = $"{officeId}:{dateKey}";

        AiUsageQuota quota = await GetQuota(id, officeId) ?? new AiUsageQuota
        {
            Id = id,
            OfficeId = officeId,
            UsageDate = dateKey,
            RequestCount = 0
        };

        quota.RequestCount += 1;

        ItemResponse<AiUsageQuota> response = await _container.UpsertItemAsync(quota, new PartitionKey(officeId));
        return response.Resource.RequestCount;
    }

    private async Task<AiUsageQuota?> GetQuota(string id, string officeId)
    {
        try
        {
            ItemResponse<AiUsageQuota> response = await _container.ReadItemAsync<AiUsageQuota>(id, new PartitionKey(officeId));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}
