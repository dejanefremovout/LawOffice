using Azure.Storage.Blobs;

namespace CaseManagement.Infrastructure.Data;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobService(BlobServiceClient blobServiceClient)
    {
        ArgumentNullException.ThrowIfNull(blobServiceClient);

        _blobServiceClient = blobServiceClient;
    }

    public BlobContainerClient BlobContainerClient(string containerName)
    {
        BlobContainerClient client = _blobServiceClient.GetBlobContainerClient(containerName);
        return client;
    }

    public BlobClient BlobClient(BlobContainerClient container, string blobName)
    {
        BlobClient client = container.GetBlobClient(blobName);
        return client;
    }

    public BlobClient BlobClient(string containerName, string blobName)
    {
        BlobClient client = BlobContainerClient(containerName).GetBlobClient(blobName);
        return client;
    }
}
