namespace Services.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;
    using Services;
    using static System.StringComparison;

    internal static class ServiceProviderExtensions
    {
        internal static IDataService GetMongoDataService(this IServiceProvider provider, string name)
        {
            var clients = provider.GetRequiredService<IDictionary<string, IMongoClient>>();
            var client = clients[name];
            var monitor = provider.GetRequiredService<IOptionsMonitor<MongoDataOptions>>();
            var options = monitor.Get(name);
            return new MongoDataService(client, options, name);
        }

        internal static T GetMongoDataService<T>(this IServiceProvider provider, string name)
        {
            var dataServices = provider.GetServices<IDataService>();
            var dataService = dataServices.Single(x => string.Equals(x.Name, name, InvariantCultureIgnoreCase));
            return (T)dataService;
        }
    }
}
