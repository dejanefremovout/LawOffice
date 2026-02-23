using Microsoft.Azure.Cosmos;
using OfficeManagement.Domain.Entities;
using OfficeManagement.Domain.Interfaces;
using OfficeManagement.Infrastructure.Data;
using System.Net;

namespace OfficeManagement.Infrastructure.Repositories;

public class OfficeRepository(ICosmosService cosmosService) : IOfficeRepository
{
    private const string _officesContainerId = "offices";
    private readonly Container _container = cosmosService.GetContainer(_officesContainerId);

    public async Task<Office> Add(Office office)
    {
        return await Upsert(office);
    }

    public async Task<Office> Update(Office office)
    {
        return await Upsert(office);
    }

    private async Task<Office> Upsert(Office office)
    {
        ArgumentNullException.ThrowIfNull(office);

        ItemResponse<Office> response = await _container.UpsertItemAsync(office, new PartitionKey(office.Id));
        return response.Resource;
    }

    public async Task<Office?> Get(string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        try
        {
            ItemResponse<Office> response = await _container.ReadItemAsync<Office>(officeId, new PartitionKey(officeId));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Office>> GetAll()
    {
        FeedIterator<Office> query = _container.GetItemQueryIterator<Office>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<Office>();

        while (query.HasMoreResults)
        {
            FeedResponse<Office> feed = await query.ReadNextAsync();
            results.AddRange(feed.Resource);
        }

        return results;
    }
}
