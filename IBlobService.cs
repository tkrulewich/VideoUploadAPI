using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public interface IBlobService
{
    Task<string> UploadFileBlobAsync(Stream content, string fileName, string contentType);
    Task DeleteBlobDataAsync(string fileName);
    Task<bool> BlobExistsAsync(string fileName);
    Task<Stream> GetBlobDataAsync(string fileName);
    Task<IEnumerable<string>> ListBlobDataAsync();
}

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobService(BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = configuration["BlobStorage:ContainerName"];
    }

    public async Task<string> UploadFileBlobAsync(Stream content, string fileName, string contentType)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });

        return blobClient.Name;
    }

    public async Task DeleteBlobDataAsync(string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<bool> BlobExistsAsync(string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        return await blobClient.ExistsAsync();
    }

    public async Task<Stream> GetBlobDataAsync(string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        var response = await blobClient.DownloadAsync();

        return response.Value.Content;
    }

    public async Task<IEnumerable<string>> ListBlobDataAsync()
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        var result = new List<string>();

        await foreach (var blob in containerClient.GetBlobsAsync())
        {
            result.Add(blob.Name);
        }

        return result;
    }
}