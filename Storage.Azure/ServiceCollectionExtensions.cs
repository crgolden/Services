namespace Services
{
    using System;
    using Common;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Auth;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureStorageService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(AzureStorageOptions));
            if (!section.Exists())
            {
                throw new Exception("AzureStorageOptions section doesn't exist");
            }

            services.Configure<AzureStorageOptions>(section);
            var azureStorageOptions = section.Get<AzureStorageOptions>();
            if (azureStorageOptions == default ||
                string.IsNullOrEmpty(azureStorageOptions.AccountName) ||
                string.IsNullOrEmpty(azureStorageOptions.AccountKey1))
            {
                throw new Exception("AzureStorageOptions section is invalid");
            }

            services.AddTransient(
                implementationFactory: sp =>
                {
                    var storageCredentials = new StorageCredentials(
                        accountName: azureStorageOptions.AccountName,
                        keyValue: azureStorageOptions.AccountKey1);
                    var storageAccount = new CloudStorageAccount(
                        storageCredentials: storageCredentials,
                        useHttps: true);
                    return storageAccount.CreateCloudBlobClient();
                });
            services.AddTransient<IStorageService, AzureStorageService>();
            return services;
        }
    }
}
