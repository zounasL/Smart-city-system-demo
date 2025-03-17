using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace Smart_City_Azure_map.Services
{
    public class BlobStorageService
    {
        private BlobServiceClient blobServiceClient;

        public BlobStorageService(IConfiguration configuration)
        {
            blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzureBlobMapLayers"));
        }

        public BlobClient getBlobClient(string container, string blob)
        {
            BlobClient blobClient = blobServiceClient.GetBlobContainerClient(container).GetBlobClient(blob);
            return blobClient;
        }

        async public Task<List<string>> getLayersList(string container) {
            List<string> blobs = new List<string>();
            // List all blobs in the container
            await foreach (BlobItem blobItem in blobServiceClient.GetBlobContainerClient(container).GetBlobsAsync())
            {
                blobs.Add(blobItem.Name);
                System.Diagnostics.Debug.WriteLine("\t" + blobItem.Name);
            }

            return blobs;
        }
    }
}
