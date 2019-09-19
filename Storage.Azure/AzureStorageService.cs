﻿namespace Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Auth;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Extensions.Options;

    public class AzureStorageService : IStorageService
    {
        private readonly CloudBlobClient _client;

        public AzureStorageService(IOptions<AzureStorageOptions> storageOptions)
        {
            var storageCredentials = new StorageCredentials(
                accountName: storageOptions.Value.AccountName,
                keyValue: storageOptions.Value.AccountKey1);
            var storageAccount = new CloudStorageAccount(
                storageCredentials: storageCredentials,
                useHttps: true);
            _client = storageAccount.CreateCloudBlobClient();
        }

        public async Task<Uri> Upload(
            Stream stream,
            string fileName,
            string containerName,
            CancellationToken cancellationToken = default)
        {
            var container = _client.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(fileName);
            await blob.UploadFromStreamAsync(stream, cancellationToken).ConfigureAwait(false);
            return blob.Uri;
        }

        public async Task Delete(
            string fileName,
            string containerName,
            CancellationToken cancellationToken = default)
        {
            var container = _client.GetContainerReference(containerName);
            var blob = await container.GetBlobReferenceFromServerAsync(fileName, cancellationToken).ConfigureAwait(false);
            await blob.DeleteIfExistsAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAll(
            string containerName,
            CancellationToken cancellationToken = default)
        {
            var container = _client.GetContainerReference(containerName);
            foreach (var blob in container.ListBlobs(null, true).OfType<CloudBlob>())
            {
                await blob.DeleteIfExistsAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public string GetUrl(
            string fileName,
            string containerName,
            DateTime expiration)
        {
            var container = _client.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(fileName);
            var policy = new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = expiration,
                Permissions = SharedAccessBlobPermissions.Read
            };
            return blob.GetSharedAccessSignature(policy);
        }
    }
}