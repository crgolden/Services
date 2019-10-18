namespace Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Extensions.Logging;

    public class AzureStorageService : IStorageService
    {
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly ILogger<AzureStorageService> _logger;

        public AzureStorageService(
            CloudBlobClient? cloudBlobClient,
            ILogger<AzureStorageService>? logger)
        {
            _cloudBlobClient = cloudBlobClient ?? throw new ArgumentNullException(nameof(cloudBlobClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Uri> Upload(
            Stream? stream,
            string? fileName,
            string? containerName,
            CancellationToken cancellationToken = default)
        {
            if (stream == default)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (string.IsNullOrEmpty(nameof(containerName)))
            {
                throw new ArgumentNullException(nameof(containerName));
            }

            var container = _cloudBlobClient.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(fileName);
            await blob.UploadFromStreamAsync(stream, cancellationToken).ConfigureAwait(false);
            return blob.Uri;
        }

        public async Task Delete(
            string? fileName,
            string? containerName,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (string.IsNullOrEmpty(nameof(containerName)))
            {
                throw new ArgumentNullException(nameof(containerName));
            }

            var container = _cloudBlobClient.GetContainerReference(containerName);
            var blob = await container.GetBlobReferenceFromServerAsync(fileName, cancellationToken).ConfigureAwait(false);
            await blob.DeleteIfExistsAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAll(
            string? containerName,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(nameof(containerName)))
            {
                throw new ArgumentNullException(nameof(containerName));
            }

            var container = _cloudBlobClient.GetContainerReference(containerName);
            foreach (var blob in container.ListBlobs(null, true).OfType<CloudBlob>())
            {
                await blob.DeleteIfExistsAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public Uri GetUrl(
            string? fileName,
            string? containerName,
            DateTime? expiration)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (string.IsNullOrEmpty(nameof(containerName)))
            {
                throw new ArgumentNullException(nameof(containerName));
            }

            if (!expiration.HasValue)
            {
                throw new ArgumentNullException(nameof(expiration));
            }

            var container = _cloudBlobClient.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(fileName);
            var policy = new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = expiration.Value,
                Permissions = SharedAccessBlobPermissions.Read
            };
            var sharedAccessSignature = blob.GetSharedAccessSignature(policy);
            return new Uri(sharedAccessSignature);
        }
    }
}
