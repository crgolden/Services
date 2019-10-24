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
    using static System.DateTime;
    using static System.String;
    using static System.Threading.Tasks.Task;
    using static Common.EventId;
    using static Microsoft.Azure.Storage.Blob.SharedAccessBlobPermissions;
    using static Microsoft.Extensions.Logging.LogLevel;
    using EventId = Microsoft.Extensions.Logging.EventId;

    /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<Uri> Upload(
            Stream? stream,
            string? fileName,
            string? containerName,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (stream == default)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName));
            }

            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageUploadStart, $"{StorageUploadStart}"),
                    message: "Uploading file {@File} to container {@Container} at {@Time}",
                    args: new object[] { fileName, containerName, UtcNow });
                var blob = _cloudBlobClient
                    .GetContainerReference(containerName)
                    .GetBlockBlobReference(fileName);
                await blob
                    .UploadFromStreamAsync(stream, cancellationToken)
                    .ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageUploadEnd, $"{StorageUploadEnd}"),
                    message: "Uploaded file {@File} to container {@Container} with uri {@Uri} at {@Time}",
                    args: new object[] { fileName, containerName, blob.Uri, UtcNow });
                return blob.Uri;
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageUploadError, $"{StorageUploadError}"),
                    exception: e,
                    message: "Error uploading file {@File} to container {@Container} at {@Time}",
                    args: new object[] { fileName, containerName, UtcNow });
                throw;
            }
        }

        /// <inheritdoc />
        public async Task Delete(
            string? fileName,
            string? containerName,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName));
            }

            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageRemoveStart, $"{StorageRemoveStart}"),
                    message: "Removing file {@File} from container {@Container} at {@Time}",
                    args: new object[] { fileName, containerName, UtcNow });
                var blob = await _cloudBlobClient
                    .GetContainerReference(containerName)
                    .GetBlobReferenceFromServerAsync(fileName, cancellationToken)
                    .ConfigureAwait(false);
                var deleted = await blob.DeleteIfExistsAsync(cancellationToken)
                    .ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageRemoveEnd, $"{StorageRemoveEnd}"),
                    message: "Removed file {@File} from container {@Container} with response {@Response} at {@Time}",
                    args: new object[] { fileName, containerName, deleted, UtcNow });
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageRemoveError, $"{StorageRemoveError}"),
                    exception: e,
                    message: "Error removing file {@File} from container {@Container} at {@Time}",
                    args: new object[] { fileName, containerName, UtcNow });
                throw;
            }
        }

        /// <inheritdoc />
        public async Task DeleteAll(
            string? containerName,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(containerName);
            }

            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageRemoveAllStart, $"{StorageRemoveAllStart}"),
                    message: "Removing from container {@Container} at {@Time}",
                    args: new object[] { containerName, UtcNow });
                var responses = await WhenAll(_cloudBlobClient
                    .GetContainerReference(containerName)
                    .ListBlobs(null, true)
                    .OfType<CloudBlob>()
                    .Select(x => x.DeleteIfExistsAsync(cancellationToken)))
                    .ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageRemoveAllEnd, $"{StorageRemoveAllEnd}"),
                    message: "Removed from container {@Container} with response {@Response} at {@Time}",
                    args: new object[] { containerName, responses, UtcNow });
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageRemoveAllError, $"{StorageRemoveAllError}"),
                    exception: e,
                    message: "Error removing from container {@Container} at {@Time}",
                    args: new object[] { containerName, UtcNow });
                throw;
            }
        }

        /// <inheritdoc />
        public Uri GetUrl(
            string? fileName,
            string? containerName,
            DateTime? expiration,
            LogLevel logLevel = Trace)
        {
            if (IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName));
            }

            if (!expiration.HasValue)
            {
                throw new ArgumentNullException(nameof(expiration));
            }

            var policy = new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = expiration.Value,
                Permissions = Read
            };
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageGetUrlStart, $"{StorageGetUrlStart}"),
                    message: "Getting policy {@Policy} for container {@Container} at {@Time}",
                    args: new object[] { policy, containerName, UtcNow });
                var blob = _cloudBlobClient
                    .GetContainerReference(containerName)
                    .GetBlockBlobReference(fileName);
                var sharedAccessSignature = blob.GetSharedAccessSignature(policy);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageGetUrlEnd, $"{StorageGetUrlEnd}"),
                    message: "Got policy {@Policy} for container {@Container} with response {@Response} at {@Time}",
                    args: new object[] { policy, containerName, sharedAccessSignature, UtcNow });
                return new Uri(sharedAccessSignature);
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageGetUrlError, $"{StorageGetUrlError}"),
                    exception: e,
                    message: "Error getting policy {@Policy} for container {@Container} at {@Time}",
                    args: new object[] { policy, containerName, UtcNow });
                throw;
            }
        }
    }
}
