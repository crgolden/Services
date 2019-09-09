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
    using Microsoft.Extensions.Options;

    public class AmazonStorageService : IStorageService
    {
        private readonly string _accessKeyId;
        private readonly string _secretAccessKey;

        public AmazonStorageService(IOptions<AmazonStorageOptions> storageOptions)
        {
            _accessKeyId = storageOptions.Value.AccessKeyId;
            _secretAccessKey = storageOptions.Value.SecretAccessKey;
        }

        public async Task<Uri> Upload(
            Stream stream,
            string key,
            string bucketName,
            CancellationToken cancellationToken = default)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = stream
            };
            using (var client = new AmazonS3Client(_accessKeyId, _secretAccessKey))
            {
                await client.PutObjectAsync(request, cancellationToken).ConfigureAwait(false);
            }

            return new Uri($"https://s3.amazonaws.com/{bucketName}/{key}");
        }

        public async Task Delete(
            string key,
            string bucketName,
            CancellationToken cancellationToken = default)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };
            using (var client = new AmazonS3Client(_accessKeyId, _secretAccessKey))
            {
                await client.DeleteObjectAsync(request, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task DeleteAll(
            string bucketName,
            CancellationToken cancellationToken = default)
        {
            var listObjectsRequest = new ListObjectsV2Request
            {
                BucketName = bucketName
            };
            var deleteObjectsRequest = new DeleteObjectsRequest
            {
                BucketName = bucketName,
            };
            using (var client = new AmazonS3Client(_accessKeyId, _secretAccessKey))
            {
                var objects = await client.ListObjectsV2Async(listObjectsRequest, cancellationToken).ConfigureAwait(false);
                deleteObjectsRequest.Objects = objects.S3Objects.Select(s3Object => new KeyVersion
                {
                    Key = s3Object.Key
                }).ToList();
                await client.DeleteObjectsAsync(deleteObjectsRequest, cancellationToken).ConfigureAwait(false);
            }
        }

        public string GetUrl(
            string key,
            string bucketName,
            DateTime expiration)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
                Expires = expiration
            };
            using (var client = new AmazonS3Client(_accessKeyId, _secretAccessKey))
            {
                return client.GetPreSignedURL(request);
            }
        }
    }
}
