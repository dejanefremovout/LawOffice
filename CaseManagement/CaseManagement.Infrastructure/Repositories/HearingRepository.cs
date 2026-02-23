using CaseManagement.Domain.Entities;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Infrastructure.Data;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace CaseManagement.Infrastructure.Repositories;

public class HearingRepository(ICosmosService cosmosService) : IHearingRepository
{
    private const string _hearingsContainerId = "hearings";
    private readonly Container _container = cosmosService.GetContainer(_hearingsContainerId);

    public async Task<Hearing> Add(Hearing hearing)
    {
        return await Upsert(hearing);
    }

    public async Task<Hearing> Update(Hearing hearing)
    {
        return await Upsert(hearing);
    }

    private async Task<Hearing> Upsert(Hearing hearing)
    {
        ArgumentNullException.ThrowIfNull(hearing);

        ItemResponse<Hearing> response = await _container.UpsertItemAsync(hearing, new PartitionKey(hearing.OfficeId));
        return response.Resource;
    }

    public async Task<Hearing?> Get(string hearingId, string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hearingId);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        try
        {
            ItemResponse<Hearing> response = await _container.ReadItemAsync<Hearing>(hearingId, new PartitionKey(officeId));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Hearing>> GetAll(string caseId, string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        FeedIterator<Hearing> query = _container.GetItemQueryIterator<Hearing>(
            new QueryDefinition("SELECT * FROM c WHERE c.officeId = @officeId AND c.caseId = @caseId")
            .WithParameter("@officeId", officeId)
            .WithParameter("@caseId", caseId));
        List<Hearing> results = [];

        while (query.HasMoreResults)
        {
            FeedResponse<Hearing> feed = await query.ReadNextAsync();
            results.AddRange(feed.Resource);
        }

        return results;
    }

    public async Task Delete(string hearingId, string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hearingId);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        try
        {
            _ = await _container.DeleteItemAsync<Hearing>(hearingId, new PartitionKey(officeId));
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
        }
    }
}
