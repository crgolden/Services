namespace Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using JetBrains.Annotations;
    using Microsoft.Azure.Storage.Blob;
    using static System.DateTime;
    using static System.String;
    using static System.Threading.Tasks.Task;
    using static Microsoft.Azure.Storage.Blob.SharedAccessBlobPermissions;

    /// <inheritdoc />
    [PublicAPI]
    public class AzureStorageService : IStorageService
    {
        private readonly CloudBlobClient _cloudBlobClient;

        /// <summary>Initializes a new instance of the <see cref="AzureStorageService"/> class.</summary>
        /// <param name="cloudBlobClient">The <see cref="CloudBlobClient"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cloudBlobClient" /> is <see langword="null" />.</exception>
        public AzureStorageService(CloudBlobClient cloudBlobClient)
        {
            _cloudBlobClient = cloudBlobClient ?? throw new ArgumentNullException(nameof(cloudBlobClient));
        }

        /// <inheritdoc />
        public Task<Uri> Upload(Stream stream, string fileName, string folderName, CancellationToken cancellationToken = default)
        {
            if (stream == default)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            async Task<Uri> Upload()
            {
                var blob = _cloudBlobClient
                    .GetContainerReference(folderName)
                    .GetBlockBlobReference(fileName);
                await blob
                    .UploadFromStreamAsync(stream, cancellationToken)
                    .ConfigureAwait(false);
                return blob.Uri;
            }

            return Upload();
        }

        /// <inheritdoc />
        public Task Delete(string fileName, string folderName, CancellationToken cancellationToken = default)
        {
            if (IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            async Task Delete()
            {
                var blob = await _cloudBlobClient
                    .GetContainerReference(folderName)
                    .GetBlobReferenceFromServerAsync(fileName, cancellationToken)
                    .ConfigureAwait(false);
                await blob.DeleteIfExistsAsync(cancellationToken).ConfigureAwait(false);
            }

            return Delete();
        }

        /// <inheritdoc />
        public Task DeleteAll(string folderName, CancellationToken cancellationToken = default)
        {
            if (IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(folderName);
            }

            return WhenAll(_cloudBlobClient
                .GetContainerReference(folderName)
                .ListBlobs(null, true)
                .OfType<CloudBlob>()
                .Select(x => x.DeleteIfExistsAsync(cancellationToken)));
        }

        /// <inheritdoc />
        public Uri GetUrl(string fileName, string folderName, DateTime expiration)
        {
            if (IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            if (expiration == default)
            {
                throw new ArgumentException("Invalid expiration", nameof(expiration));
            }

            var policy = new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = expiration,
                Permissions = Read
            };
            var blob = _cloudBlobClient
                .GetContainerReference(folderName)
                .GetBlockBlobReference(fileName);
            var sharedAccessSignature = blob.GetSharedAccessSignature(policy);
            return new Uri(sharedAccessSignature);
        }
    }
}
