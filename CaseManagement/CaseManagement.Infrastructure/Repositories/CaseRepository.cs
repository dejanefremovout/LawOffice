using CaseManagement.Domain.Entities;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Infrastructure.Data;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace CaseManagement.Infrastructure.Repositories;

public class CaseRepository(ICosmosService cosmosService) : ICaseRepository
{
    private const string _casesContainerId = "cases";
    private readonly Container _container = cosmosService.GetContainer(_casesContainerId);

    public async Task<Case> Add(Case caseItem)
    {
        return await Upsert(caseItem);
    }

    public async Task<Case> Update(Case caseItem)
    {
        return await Upsert(caseItem);
    }

    private async Task<Case> Upsert(Case caseItem)
    {
        ArgumentNullException.ThrowIfNull(caseItem);

        ItemResponse<Case> response = await _container.UpsertItemAsync(caseItem, new PartitionKey(caseItem.OfficeId));
        return response.Resource;
    }

    public async Task<Case?> Get(string caseId, string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(caseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        try
        {
            ItemResponse<Case> response = await _container.ReadItemAsync<Case>(caseId, new PartitionKey(officeId));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Case>> GetAll(string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        FeedIterator<Case> query = _container.GetItemQueryIterator<Case>(
            new QueryDefinition("SELECT * FROM c WHERE c.officeId = @officeId")
            .WithParameter("@officeId", officeId));
        List<Case> results = [];

        while (query.HasMoreResults)
        {
            FeedResponse<Case> feed = await query.ReadNextAsync();
            results.AddRange(feed.Resource);
        }

        return results;
    }

    public async Task<IEnumerable<Case>> GetByIds(string officeId, IEnumerable<string> caseIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentNullException.ThrowIfNull(caseIds);

        List<string> caseIdList = caseIds.ToList();
        if (!caseIdList.Any())
        {
            return [];
        }

        var idPlaceholders = string.Join(",", Enumerable.Range(0, caseIdList.Count).Select(i => $"@id{i}"));
        var queryDefinition = new QueryDefinition(
            $"SELECT * FROM c WHERE c.officeId = @officeId AND c.id IN ({idPlaceholders})")
            .WithParameter("@officeId", officeId);

        for (int i = 0; i < caseIdList.Count; i++)
        {
            _ = queryDefinition.WithParameter($"@id{i}", caseIdList[i]);
        }

        var query = _container.GetItemQueryIterator<Case>(queryDefinition);
        List<Case> results = [];

        while (query.HasMoreResults)
        {
            var feed = await query.ReadNextAsync();
            results.AddRange(feed.Resource);
        }

        return results;
    }

    public async Task Delete(string caseId, string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(caseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        try
        {
            _ = await _container.DeleteItemAsync<Case>(caseId, new PartitionKey(officeId));
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
        }
    }

    public async Task<int> GetTotalCount(string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        var queryDefinition = new QueryDefinition(
            "SELECT VALUE COUNT(1) FROM c WHERE c.officeId = @officeId")
            .WithParameter("@officeId", officeId);

        var query = _container.GetItemQueryIterator<int>(queryDefinition);

        while (query.HasMoreResults)
        {
            var feed = await query.ReadNextAsync();
            if (feed.Resource.Any())
            {
                return feed.Resource.First();
            }
        }

        return 0;
    }

    public async Task<int> GetActiveCount(string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        var queryDefinition = new QueryDefinition(
            "SELECT VALUE COUNT(1) FROM c WHERE c.officeId = @officeId AND c.active = true")
            .WithParameter("@officeId", officeId);

        var query = _container.GetItemQueryIterator<int>(queryDefinition);

        while (query.HasMoreResults)
        {
            var feed = await query.ReadNextAsync();
            if (feed.Resource.Any())
            {
                return feed.Resource.First();
            }
        }

        return 0;
    }

    public async Task<IEnumerable<Case>> GetLastActiveCases(string officeId, int count)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        var queryDefinition = new QueryDefinition(
            "SELECT TOP @count * FROM c WHERE c.officeId = @officeId AND c.active = true ORDER BY c._ts DESC")
            .WithParameter("@officeId", officeId)
            .WithParameter("@count", count);

        var query = _container.GetItemQueryIterator<Case>(queryDefinition);
        List<Case> results = [];

        while (query.HasMoreResults)
        {
            var feed = await query.ReadNextAsync();
            results.AddRange(feed.Resource);
        }

        return results;
    }
}
