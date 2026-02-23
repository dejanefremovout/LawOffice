using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Infrastructure.Data;

namespace CaseManagement.Infrastructure.Repositories;

public class DocumentFileStorageRepository(IBlobService blobService) : IDocumentFileStorageRepository
{
    public async Task<Uri> GenerateUploadUri(string officeId, string documentFileId)
    {
        BlobContainerClient blobContainerClient = blobService.BlobContainerClient(officeId);
        _ = await blobContainerClient.CreateIfNotExistsAsync();
        BlobClient blobClient = blobService.BlobClient(blobContainerClient, documentFileId);

        BlobSasBuilder sasBuilder = new()
        {
            BlobContainerName = officeId,
            BlobName = documentFileId,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Write | BlobSasPermissions.Create);

        Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri;
    }

    public async Task<Uri> GenerateDownloadUri(string officeId, string documentFileId)
    {
        BlobContainerClient blobContainerClient = blobService.BlobContainerClient(officeId);
        _ = await blobContainerClient.CreateIfNotExistsAsync();
        BlobClient blobClient = blobService.BlobClient(blobContainerClient, documentFileId);

        BlobSasBuilder sasBuilder = new()
        {
            BlobContainerName = officeId,
            BlobName = documentFileId,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri;
    }
}
