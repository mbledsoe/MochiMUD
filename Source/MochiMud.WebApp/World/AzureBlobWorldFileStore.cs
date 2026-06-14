using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace MochiMud.WebApp.World
{
    public class AzureBlobWorldFileStore : IWorldFileStore
    {
        private readonly BlobContainerClient containerClient;

        public AzureBlobWorldFileStore(IOptions<WorldFileStorageOptions> options)
        {
            var storageOptions = options.Value.AzureBlobStorage;

            containerClient = !string.IsNullOrWhiteSpace(storageOptions.ConnectionString)
                ? new BlobContainerClient(storageOptions.ConnectionString, storageOptions.ContainerName)
                : new BlobServiceClient(
                    storageOptions.ServiceUri
                        ?? throw new InvalidOperationException("Azure blob world storage requires a service URI or connection string."),
                    new DefaultAzureCredential()).GetBlobContainerClient(storageOptions.ContainerName);
        }

        public async Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            var blobName = path.Replace('\\', '/');
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!(await blobClient.ExistsAsync(cancellationToken)).Value)
            {
                throw new FileNotFoundException("World data blob was not found.", blobName);
            }

            return await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        }
    }
}
