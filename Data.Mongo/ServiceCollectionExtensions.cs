namespace Services
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Driver;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDataService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(MongoDataOptions));
            if (!section.Exists())
            {
                throw new Exception("MongoDataOptions section doesn't exist");
            }

            services.Configure<MongoDataOptions>(section);
            var mongoDataOptions = section.Get<MongoDataOptions>();
            if (mongoDataOptions == default ||
                string.IsNullOrEmpty(mongoDataOptions.ConnectionString) ||
                string.IsNullOrEmpty(mongoDataOptions.DatabaseName))
            {
                throw new Exception("MongoDataOptions section is invalid");
            }

            services.AddSingleton<IMongoClient>(
                implementationFactory: sp => new MongoClient(mongoDataOptions.ConnectionString));
            services.AddScoped(
                implementationFactory: sp =>
                {
                    var mongoClient = sp.GetRequiredService<IMongoClient>();
                    return mongoClient.GetDatabase(mongoDataOptions.DatabaseName);
                });
            return services;
        }
    }
}
