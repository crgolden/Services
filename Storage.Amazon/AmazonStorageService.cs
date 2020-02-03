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
    using JetBrains.Annotations;
    using static System.String;

    /// <inheritdoc />
    [PublicAPI]
    public class AmazonStorageService : IStorageService
    {
        private readonly IAmazonS3 _amazonS3;

        /// <summary>Initializes a new instance of the <see cref="AmazonStorageService"/> class.</summary>
        /// <param name="amazonS3">The <seealso cref="IAmazonS3"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="amazonS3" /> is <see langword="null" />.</exception>
        public AmazonStorageService(IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3 ?? throw new ArgumentNullException(nameof(amazonS3));
        }

        /// <inheritdoc />
        public Task<Uri> Upload(
            Stream stream,
            string fileName,
            string folderName,
            CancellationToken cancellationToken = default)
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
                var putObjectRequest = new PutObjectRequest
                {
                    BucketName = folderName,
                    Key = fileName,
                    InputStream = stream
                };
                await _amazonS3
                    .PutObjectAsync(putObjectRequest, cancellationToken)
                    .ConfigureAwait(false);
                return new Uri($"https://s3.amazonaws.com/{folderName}/{fileName}");
            }

            return Upload();
        }

        /// <inheritdoc />
        public Task Delete(
            string fileName,
            string folderName,
            CancellationToken cancellationToken = default)
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
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = folderName,
                    Key = fileName
                };
                await _amazonS3
                    .DeleteObjectAsync(deleteObjectRequest, cancellationToken)
                    .ConfigureAwait(false);
            }

            return Delete();
        }

        /// <inheritdoc />
        public Task DeleteAll(
            string folderName,
            CancellationToken cancellationToken = default)
        {
            if (IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            async Task DeleteAll()
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
                await _amazonS3
                    .DeleteObjectsAsync(deleteObjectsRequest, cancellationToken)
                    .ConfigureAwait(false);
            }

            return DeleteAll();
        }

        /// <inheritdoc />
        public Uri GetUrl(
            string fileName,
            string folderName,
            DateTime expiration)
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

            var getPreSignedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = folderName,
                Key = fileName,
                Expires = expiration
            };
            var preSignedUrl = _amazonS3.GetPreSignedURL(getPreSignedUrlRequest);
            return new Uri(preSignedUrl);
        }
    }
}
