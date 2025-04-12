using Azure.Storage.Blobs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Services;
public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobService()
    {
        var connectionString = "DefaultEndpointsProtocol=https;AccountName=fitnessproimages;AccountKey=mhwZZ1hSJ4f2ArUXwpOX/0Ps81YYeEjydbQS6MJ+fvNShekX8w4bEp7iQ8R8EdoisWPRsUQHW1vU+AStrghpzQ==;EndpointSuffix=core.windows.net";
        _containerName = "images";

        // Create BlobServiceClient using ConnectionString
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    // Upload image to Blob Storage
    public async Task<string> UploadImageAsync(IFormFile file)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = blobContainerClient.GetBlobClient(file.FileName);

        // Upload the image
        await blobClient.UploadAsync(file.OpenReadStream(), true);
        return blobClient.Uri.ToString();
    }

    // Delete image from Blob Storage
    public async Task DeleteImageAsync(string imageUrl)
    {
        var uri = new Uri(imageUrl);
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = blobContainerClient.GetBlobClient(uri.AbsolutePath.TrimStart('/'));

        // Delete the image
        await blobClient.DeleteIfExistsAsync();
    }
}
