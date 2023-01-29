namespace Services
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Common.Services;
    using JetBrains.Annotations;
    using static System.DateTime;
    using static System.String;
    using static Azure.Storage.Sas.BlobSasPermissions;

    /// <inheritdoc />
    [PublicAPI]
    public class AzureStorageService : IStorageService
    {
        private readonly BlobServiceClient _cloudBlobClient;

        /// <summary>Initializes a new instance of the <see cref="AzureStorageService"/> class.</summary>
        /// <param name="blobServiceClient">The <see cref="BlobServiceClient"/>.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="blobServiceClient" /> is <see langword="null" />
        /// or
        /// <paramref name="name" /> is <see langword="null" />.</exception>
        public AzureStorageService(
            BlobServiceClient blobServiceClient,
            string name = nameof(AzureStorageService))
        {
            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            _cloudBlobClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
            Name = name;
        }

        /// <inheritdoc />
        public string Name { get; }

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

            async Task<Uri> UploadAsync()
            {
                var blob = _cloudBlobClient
                    .GetBlobContainerClient(folderName)
                    .GetBlobClient(fileName);
                await blob
                    .UploadAsync(stream, cancellationToken)
                    .ConfigureAwait(false);
                return blob.Uri;
            }

            return UploadAsync();
        }

        /// <inheritdoc />
        public Task Delete(string fileName, string folderName, CancellationToken cancellationToken = default)
        {
            return DeleteAll(folderName, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteAll(string folderName, CancellationToken cancellationToken = default)
        {
            if (IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(folderName);
            }

            var blobContainerClient = _cloudBlobClient.GetBlobContainerClient(folderName);
            var blobItems = blobContainerClient.GetBlobsAsync(prefix: folderName, cancellationToken: cancellationToken);

            async Task DeleteAllAsync()
            {
                await foreach (var blobItem in blobItems)
                {
                    var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                    await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                }
            }

            return DeleteAllAsync();
        }

        /// <inheritdoc />
        public Uri GetUrl(string fileName, string folderName, DateTimeOffset expiration)
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

            var blobClient = _cloudBlobClient
                .GetBlobContainerClient(folderName)
                .GetBlobClient(fileName);
            return blobClient.GenerateSasUri(Read, UtcNow.AddHours(1));
        }
    }
}
