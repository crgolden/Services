﻿namespace Services
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

        public async Task<Uri> Upload(
            Stream? stream,
            string? key,
            string? bucketName,
            CancellationToken cancellationToken = default)
        {
            if (stream == default)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException(nameof(bucketName));
            }

            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = stream
            };
            await _amazonS3.PutObjectAsync(request, cancellationToken).ConfigureAwait(false);
            return new Uri($"https://s3.amazonaws.com/{bucketName}/{key}");
        }

        public async Task Delete(
            string? key,
            string? bucketName,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException(nameof(bucketName));
            }

            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };
            await _amazonS3.DeleteObjectAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAll(
            string? bucketName,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException(nameof(bucketName));
            }

            var listObjectsRequest = new ListObjectsV2Request
            {
                BucketName = bucketName
            };
            var objects = await _amazonS3.ListObjectsV2Async(listObjectsRequest, cancellationToken).ConfigureAwait(false);
            var deleteObjectsRequest = new DeleteObjectsRequest
            {
                BucketName = bucketName,
                Objects = objects.S3Objects.Select(s3Object => new KeyVersion
                {
                    Key = s3Object.Key
                }).ToList()
            };
            await _amazonS3.DeleteObjectsAsync(deleteObjectsRequest, cancellationToken).ConfigureAwait(false);
        }

        public Uri GetUrl(
            string? key,
            string? bucketName,
            DateTime? expiration)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException(nameof(bucketName));
            }

            if (!expiration.HasValue)
            {
                throw new ArgumentNullException(nameof(expiration));
            }

            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
                Expires = expiration.Value
            };
            var preSignedUrl = _amazonS3.GetPreSignedURL(request);
            return new Uri(preSignedUrl);
        }
    }
}
