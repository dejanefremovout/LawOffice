using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace CaseManagement.Infrastructure.Repositories;

public class DocumentFileStorageRepository(IBlobService blobService, IConfiguration configuration) : IDocumentFileStorageRepository
{
    private const string PublicSasBaseUriSetting = "BlobSettings:PublicSasBaseUri";
    private readonly Uri? _publicSasBaseUri = ParsePublicSasBaseUri(configuration[PublicSasBaseUriSetting]);

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
        return RewriteForPublicEndpoint(sasUri);
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
        return RewriteForPublicEndpoint(sasUri);
    }

    private static Uri? ParsePublicSasBaseUri(string? publicSasBaseUriValue)
    {
        if (string.IsNullOrWhiteSpace(publicSasBaseUriValue))
        {
            return null;
        }

        if (!Uri.TryCreate(publicSasBaseUriValue, UriKind.Absolute, out Uri? parsedUri))
        {
            throw new ArgumentException($"{PublicSasBaseUriSetting} must be an absolute URI when configured.");
        }

        return parsedUri;
    }

    private Uri RewriteForPublicEndpoint(Uri sasUri)
    {
        if (_publicSasBaseUri is null)
        {
            return sasUri;
        }

        UriBuilder uriBuilder = new(sasUri)
        {
            Scheme = _publicSasBaseUri.Scheme,
            Host = _publicSasBaseUri.Host,
            Port = _publicSasBaseUri.IsDefaultPort ? -1 : _publicSasBaseUri.Port
        };

        return uriBuilder.Uri;
    }
}
