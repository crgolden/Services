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
        public async Task<Uri> Upload(
            Stream? stream,
            string? key,
            string? bucketName,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (stream == default)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException(nameof(bucketName));
            }

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
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
                return new Uri($"https://s3.amazonaws.com/{bucketName}/{key}");
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

        /// <inheritdoc />
        public async Task Delete(
            string? key,
            string? bucketName,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException(nameof(bucketName));
            }

            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
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

        /// <inheritdoc />
        public async Task DeleteAll(
            string? bucketName,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException(nameof(bucketName));
            }

            var listObjectsRequest = new ListObjectsV2Request
            {
                BucketName = bucketName
            };
            var listObjectsResponse = await _amazonS3
                .ListObjectsV2Async(listObjectsRequest, cancellationToken)
                .ConfigureAwait(false);
            var deleteObjectsRequest = new DeleteObjectsRequest
            {
                BucketName = bucketName,
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

        /// <inheritdoc />
        public Uri GetUrl(
            string? key,
            string? bucketName,
            DateTime? expiration,
            LogLevel logLevel = Trace)
        {
            if (IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException(nameof(bucketName));
            }

            if (!expiration.HasValue)
            {
                throw new ArgumentNullException(nameof(expiration));
            }

            var getPreSignedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
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
    }
}
