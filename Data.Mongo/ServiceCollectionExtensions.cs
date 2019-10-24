namespace Services
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Driver;
    using static System.String;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDataService(
            this IServiceCollection services,
            IConfiguration? configuration)
        {
            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection(nameof(MongoDataOptions));
            if (!section.Exists())
            {
                throw new ArgumentException(
                    message: $"{nameof(MongoDataOptions)} section doesn't exist",
                    paramName: nameof(configuration));
            }

            services.Configure<MongoDataOptions>(section);
            var options = section.Get<MongoDataOptions>();
            if (options == default ||
                IsNullOrEmpty(options.ConnectionString) ||
                IsNullOrEmpty(options.DatabaseName))
            {
                throw new ArgumentException(
                    message: $"{nameof(MongoDataOptions)} section is invalid",
                    paramName: nameof(configuration));
            }

            services.AddSingleton<IMongoClient>(
                implementationFactory: sp => new MongoClient(options.ConnectionString));
            services.AddScoped(
                implementationFactory: sp =>
                {
                    var mongoClient = sp.GetRequiredService<IMongoClient>();
                    return mongoClient.GetDatabase(options.DatabaseName);
                });
            return services;
        }
    }
}
