using CaseManagement.Domain.Entities;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Infrastructure.Data;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace CaseManagement.Infrastructure.Repositories;

public class DocumentFileRepository(ICosmosService cosmosService) : IDocumentFileRepository
{
    private const string _documentFilesContainerId = "documentfiles";
    private readonly Container _container = cosmosService.GetContainer(_documentFilesContainerId);

    public async Task<DocumentFile> Add(DocumentFile documentFile)
    {
        return await Upsert(documentFile);
    }

    public async Task<DocumentFile> Update(DocumentFile documentFile)
    {
        return await Upsert(documentFile);
    }

    private async Task<DocumentFile> Upsert(DocumentFile documentFile)
    {
        ArgumentNullException.ThrowIfNull(documentFile);

        ItemResponse<DocumentFile> response = await _container.UpsertItemAsync(documentFile, new PartitionKey(documentFile.OfficeId));
        return response.Resource;
    }

    public async Task<DocumentFile?> Get(string documentFileId, string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentFileId);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        try
        {
            ItemResponse<DocumentFile> response = await _container.ReadItemAsync<DocumentFile>(documentFileId, new PartitionKey(officeId));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<DocumentFile>> GetAll(string caseId, string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(caseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        FeedIterator<DocumentFile> query = _container.GetItemQueryIterator<DocumentFile>(
            new QueryDefinition("SELECT * FROM c WHERE c.officeId = @officeId AND c.caseId = @caseId")
            .WithParameter("@officeId", officeId)
            .WithParameter("@caseId", caseId));
        List<DocumentFile> results = [];

        while (query.HasMoreResults)
        {
            FeedResponse<DocumentFile> feed = await query.ReadNextAsync();
            results.AddRange(feed.Resource);
        }

        return results;
    }

    public async Task Delete(string documentFileId, string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentFileId);
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);

        try
        {
            _ = await _container.DeleteItemAsync<DocumentFile>(documentFileId, new PartitionKey(officeId));
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
        }
    }
}
