using Azure.Storage.Blobs;

namespace CaseManagement.Infrastructure.Data;

public interface IBlobService
{
    BlobContainerClient BlobContainerClient(string containerName);
    BlobClient BlobClient(BlobContainerClient container, string blobName);
    BlobClient BlobClient(string containerName, string blobName);
}
