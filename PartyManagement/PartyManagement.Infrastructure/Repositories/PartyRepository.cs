using Microsoft.Azure.Cosmos;
using PartyManagement.Domain.Entities;
using PartyManagement.Domain.Interfaces;
using PartyManagement.Infrastructure.Data;
using System.Net;

namespace PartyManagement.Infrastructure.Repositories;

public abstract class PartyRepository : IPartyRepository
{
    protected abstract string PartiesContainerId { get; }

    private readonly Container _container;

    public PartyRepository(ICosmosService cosmosService)
    {
        _container = cosmosService.GetContainer(PartiesContainerId);
    }

    public async Task<Party> Add(Party party)
    {
        return await Upsert(party);
    }

    public async Task<Party> Update(Party party)
    {
        return await Upsert(party);
    }

    private async Task<Party> Upsert(Party party)
    {
        ArgumentNullException.ThrowIfNull(party);

        var response = await _container.UpsertItemAsync(party, new PartitionKey(party.OfficeId));
        return response.Resource;
    }

    public async Task<Party?> Get(string partyId, string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(partyId);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        try
        {
            var response = await _container.ReadItemAsync<Party>(partyId, new PartitionKey(officeId));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<bool> ExistByIdentificationNumber(string officeId, string identificationNumber, string? currentPartyId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(identificationNumber);

        var queryDefinitionText = "SELECT VALUE COUNT(1) FROM c WHERE c.officeId = @officeId AND c.identificationNumber = @identificationNumber";
        if (!string.IsNullOrWhiteSpace(currentPartyId))
        {
            queryDefinitionText += " AND c.id != @currentPartyId";
        }

        var queryDefinition = new QueryDefinition(queryDefinitionText)
            .WithParameter("@officeId", officeId)
            .WithParameter("@identificationNumber", identificationNumber);

        if (!string.IsNullOrWhiteSpace(currentPartyId))
        {
            queryDefinition = queryDefinition.WithParameter("@currentPartyId", currentPartyId);
        }

        var query = _container.GetItemQueryIterator<int>(queryDefinition);

        while (query.HasMoreResults)
        {
            var feed = await query.ReadNextAsync();
            if (feed.Resource.Any())
            {
                var count = feed.Resource.First();
                return count > 0;
            }
        }

        return false;
    }

    public async Task<IEnumerable<Party>> GetAll(string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        var queryDefinition = new QueryDefinition(
            "SELECT * FROM c WHERE c.officeId = @officeId")
            .WithParameter("@officeId", officeId);

        var requestOptions = new QueryRequestOptions
        {
            PartitionKey = new PartitionKey(officeId)
        };

        var query = _container.GetItemQueryIterator<Party>(
            queryDefinition,
            requestOptions: requestOptions);

        var results = new List<Party>();
        while (query.HasMoreResults)
        {
            var feed = await query.ReadNextAsync();
            if (feed?.Resource != null)
            {
                results.AddRange(feed.Resource);
            }
        }

        return results;
    }
}
