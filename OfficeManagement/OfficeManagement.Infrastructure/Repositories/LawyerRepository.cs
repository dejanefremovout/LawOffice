using Microsoft.Azure.Cosmos;
using OfficeManagement.Domain.Entities;
using OfficeManagement.Domain.Interfaces;
using OfficeManagement.Infrastructure.Data;
using System.Net;

namespace OfficeManagement.Infrastructure.Repositories;

public class LawyerRepository(ICosmosService cosmosService) : ILawyerRepository
{
    private const string _lawyersContainerId = "lawyers";
    private readonly Container _container = cosmosService.GetContainer(_lawyersContainerId);

    public async Task<Lawyer> Add(Lawyer lawyer)
    {
        return await Upsert(lawyer);
    }

    public async Task<Lawyer> Update(Lawyer lawyer)
    {
        return await Upsert(lawyer);
    }

    private async Task<Lawyer> Upsert(Lawyer lawyer)
    {
        ArgumentNullException.ThrowIfNull(lawyer);

        var response = await _container.UpsertItemAsync(lawyer, new PartitionKey(lawyer.OfficeId));
        return response.Resource;
    }

    public async Task<Lawyer?> Get(string lawyerId, string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lawyerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        try
        {
            var response = await _container.ReadItemAsync<Lawyer>(lawyerId, new PartitionKey(officeId));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Lawyer>> GetAll(string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        FeedIterator<Lawyer> query = _container.GetItemQueryIterator<Lawyer>(
            new QueryDefinition("SELECT * FROM c WHERE c.officeId = @officeId")
            .WithParameter("@officeId", officeId));
        List<Lawyer> results = [];

        while (query.HasMoreResults)
        {
            FeedResponse<Lawyer> feed = await query.ReadNextAsync();
            results.AddRange(feed.Resource);
        }

        return results;
    }

    public async Task<Lawyer?> GetByEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var query = _container.GetItemQueryIterator<Lawyer>(new QueryDefinition("SELECT * FROM c WHERE c.email = @email")
            .WithParameter("@email", email));

        while (query.HasMoreResults)
        {
            var feed = await query.ReadNextAsync();
            var item = feed.Resource.SingleOrDefault();
            if (item is not null)
            {
                return item;
            }
        }

        return null;
    }
}
