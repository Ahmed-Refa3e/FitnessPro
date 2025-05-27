using Azure.Storage.Blobs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Services;
public class BlobService(string connectionString) : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient = new(connectionString);
    private readonly string _containerName = "images";

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        var extension = Path.GetExtension(file.FileName);
        var blobName = $"{Guid.NewGuid()}_{Path.GetFileNameWithoutExtension(file.FileName)}{extension}";
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(file.OpenReadStream(), overwrite: true);
        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return false;

        try
        {
            var blobName = Path.GetFileName(new Uri(imageUrl).LocalPath);
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.DeleteIfExistsAsync();
            return response;
        }
        catch
        {
            return false;
        }
    }
}
