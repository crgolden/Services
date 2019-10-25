namespace Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Common;
    using Microsoft.Extensions.Logging;
    using static System.DateTime;
    using static System.String;
    using static Common.EventId;
    using static Microsoft.Extensions.Logging.LogLevel;
    using EventId = Microsoft.Extensions.Logging.EventId;

    /// <inheritdoc />
    public class AmazonStorageService : IStorageService
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly ILogger<AmazonStorageService> _logger;

        public AmazonStorageService(
            IAmazonS3? amazonS3,
            ILogger<AmazonStorageService>? logger)
        {
            _amazonS3 = amazonS3 ?? throw new ArgumentNullException(nameof(amazonS3));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public Task<Uri> Upload(
            Stream? stream,
            string? fileName,
            string? folderName,
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

            if (IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            return UploadAsync(stream, fileName, folderName, logLevel, cancellationToken);
        }

        /// <inheritdoc />
        public Task Delete(
            string? fileName,
            string? folderName,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            return DeleteAsync(fileName, folderName, logLevel, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteAll(
            string? folderName,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            return DeleteAllAsync(folderName, logLevel, cancellationToken);
        }

        /// <inheritdoc />
        public Uri GetUrl(
            string? fileName,
            string? folderName,
            DateTime? expiration,
            LogLevel logLevel = Trace)
        {
            if (IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            if (!expiration.HasValue)
            {
                throw new ArgumentNullException(nameof(expiration));
            }

            var getPreSignedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = folderName,
                Key = fileName,
                Expires = expiration.Value
            };
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageGetUrlStart, $"{StorageGetUrlStart}"),
                    message: "Getting request {@Request} at {@Time}",
                    args: new object[] { getPreSignedUrlRequest, UtcNow });
                var preSignedUrl = _amazonS3.GetPreSignedURL(getPreSignedUrlRequest);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageGetUrlEnd, $"{StorageGetUrlEnd}"),
                    message: "Got request {@Request} with response {@Response} at {@Time}",
                    args: new object[] { getPreSignedUrlRequest, preSignedUrl, UtcNow });
                return new Uri(preSignedUrl);
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageGetUrlError, $"{StorageGetUrlError}"),
                    exception: e,
                    message: "Error getting request {@Request} at {@Time}",
                    args: new object[] { getPreSignedUrlRequest, UtcNow });
                throw;
            }
        }

        private async Task<Uri> UploadAsync(
            Stream stream,
            string fileName,
            string folderName,
            LogLevel logLevel,
            CancellationToken cancellationToken)
        {
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = folderName,
                Key = fileName,
                InputStream = stream
            };
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageUploadStart, $"{StorageUploadStart}"),
                    message: "Uploading request {@Request} at {@Time}",
                    args: new object[] { putObjectRequest, UtcNow });
                var putObjectResponse = await _amazonS3
                    .PutObjectAsync(putObjectRequest, cancellationToken)
                    .ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageUploadEnd, $"{StorageUploadEnd}"),
                    message: "Uploaded request {@Request} with response {@Response} at {@Time}",
                    args: new object[] { putObjectRequest, putObjectResponse, UtcNow });
                return new Uri($"https://s3.amazonaws.com/{folderName}/{fileName}");
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageUploadError, $"{StorageUploadError}"),
                    exception: e,
                    message: "Error uploading request {@Request} at {@Time}",
                    args: new object[] { putObjectRequest, UtcNow });
                throw;
            }
        }

        private async Task DeleteAsync(
            string fileName,
            string folderName,
            LogLevel logLevel,
            CancellationToken cancellationToken)
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = folderName,
                Key = fileName
            };
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageRemoveStart, $"{StorageRemoveStart}"),
                    message: "Removing request {@Request} at {@Time}",
                    args: new object[] { deleteObjectRequest, UtcNow });
                var deleteObjectResponse = await _amazonS3
                    .DeleteObjectAsync(deleteObjectRequest, cancellationToken)
                    .ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageRemoveEnd, $"{StorageRemoveEnd}"),
                    message: "Removed request {@Request} with response {@Response} at {@Time}",
                    args: new object[] { deleteObjectRequest, deleteObjectResponse, UtcNow });
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageRemoveError, $"{StorageRemoveError}"),
                    exception: e,
                    message: "Error removing request {@Request} at {@Time}",
                    args: new object[] { deleteObjectRequest, UtcNow });
                throw;
            }
        }

        private async Task DeleteAllAsync(
            string folderName,
            LogLevel logLevel,
            CancellationToken cancellationToken)
        {
            var listObjectsRequest = new ListObjectsV2Request
            {
                BucketName = folderName
            };
            var listObjectsResponse = await _amazonS3
                .ListObjectsV2Async(listObjectsRequest, cancellationToken)
                .ConfigureAwait(false);
            var deleteObjectsRequest = new DeleteObjectsRequest
            {
                BucketName = folderName,
                Objects = listObjectsResponse.S3Objects.Select(s3Object => new KeyVersion
                {
                    Key = s3Object.Key
                }).ToList()
            };
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageRemoveAllStart, $"{StorageRemoveAllStart}"),
                    message: "Removing request {@Request} at {@Time}",
                    args: new object[] { deleteObjectsRequest, UtcNow });
                var deleteObjectsResponse = await _amazonS3
                    .DeleteObjectsAsync(deleteObjectsRequest, cancellationToken)
                    .ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageRemoveAllEnd, $"{StorageRemoveAllEnd}"),
                    message: "Removed request {@Request} with response {@Response} at {@Time}",
                    args: new object[] { deleteObjectsRequest, deleteObjectsResponse, UtcNow });
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)StorageRemoveAllError, $"{StorageRemoveAllError}"),
                    exception: e,
                    message: "Error removing request {@Request} at {@Time}",
                    args: new object[] { deleteObjectsRequest, UtcNow });
                throw;
            }
        }
    }
}
