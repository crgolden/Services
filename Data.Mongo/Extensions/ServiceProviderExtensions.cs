namespace System
{
    using Collections.Generic;
    using Common.Services;
    using JetBrains.Annotations;
    using Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using Services;
    using Threading;
    using Threading.Tasks;
    using static MongoDB.Bson.Serialization.BsonClassMap;
    using static Services.Constants.ExceptionMessages;
    using static String;
    using static StringComparison;
    using static Threading.Tasks.Task;

    /// <summary>A class with methods that extend <see cref="IServiceProvider"/>.</summary>
    [PublicAPI]
    public static class ServiceProviderExtensions
    {
        /// <summary>Registers all <paramref name="keyValuePairs"/> values and adds all <paramref name="keyValuePairs"/> keys to the <see cref="MongoDataOptions"/> instance identified by <paramref name="name"/>.</summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> instance.</param>
        /// <param name="keyValuePairs">The collection name/class map pairs.</param>
        /// <param name="name">The name of the <see cref="MongoDataOptions"/> instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <paramref name="provider"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> is <see langword="null" /> or <paramref name="keyValuePairs"/> is <see langword="null" /> or <paramref name="name"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">No <see cref="IMongoClient"/> has been added to the <paramref name="provider"/>.</exception>
        public static Task<IServiceProvider> InitializeCollectionsAsync(
            this IServiceProvider provider,
            IDictionary<string, BsonClassMap> keyValuePairs,
            string name = nameof(MongoDataService),
            CancellationToken cancellationToken = default)
        {
            if (provider == default)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (keyValuePairs == default)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }

            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var clients = provider.GetRequiredService<IDictionary<string, IMongoClient>>();
            if (!clients.ContainsKey(name))
            {
                throw new ArgumentException(ClientNotRegistered(name), nameof(name));
            }

            var monitor = provider.GetRequiredService<IOptionsMonitor<MongoDataOptions>>();
            var options = monitor.Get(name);
            foreach (var keyValuePair in keyValuePairs)
            {
                if (!IsClassMapRegistered(keyValuePair.Value.ClassType))
                {
                    RegisterClassMap(keyValuePair.Value);
                }

                if (options.CollectionNames.ContainsKey(keyValuePair.Value.ClassType.Name))
                {
                    continue;
                }

                var added = false;
                while (!added)
                {
                    added = options.CollectionNames.TryAdd(keyValuePair.Value.ClassType.Name, keyValuePair.Key);
                }
            }

            var client = clients[name];
            var database = client.GetDatabase(options.DatabaseName, options.DatabaseSettings);

            async Task<IServiceProvider> InitializeCollections()
            {
                var cursor = await database.ListCollectionNamesAsync(options.ListCollectionNamesOptions, cancellationToken).ConfigureAwait(false);
                var collectionNames = await cursor.ToListAsync(cancellationToken).ConfigureAwait(false);
                var tasks = options.CollectionNames.Values
                    .Where(x => !IsNullOrWhiteSpace(x) && collectionNames.All(y => !string.Equals(y, x, Ordinal)))
                    .Select(x => database.CreateCollectionAsync(x, options.CreateCollectionOptions, cancellationToken));
                await WhenAll(tasks).ConfigureAwait(false);
                return provider;
            }

            return InitializeCollections();
        }

        /// <summary>Builds the indexes specified in <paramref name="indexModels"/>.</summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> instance.</param>
        /// <param name="indexModels">The sequence of <see cref="CreateIndexModel{TDocument}"/>.</param>
        /// <param name="name">The name of the <see cref="MongoDataOptions"/> instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="TDocument">The type of the <paramref name="indexModels" />.</typeparam>
        /// <returns>The <paramref name="provider"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> is <see langword="null" /> or <paramref name="indexModels"/> is <see langword="null" /> or <paramref name="name"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">No <see cref="IMongoClient"/> has been added to the <paramref name="provider"/>
        /// -or-
        /// collection name not found for <typeparamref name="TDocument"/>.</exception>
        public static Task<IServiceProvider> BuildIndexesAsync<TDocument>(
            this IServiceProvider provider,
            IEnumerable<CreateIndexModel<TDocument>> indexModels,
            string name = nameof(MongoDataService),
            CancellationToken cancellationToken = default)
        {
            if (provider == default)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (indexModels == default)
            {
                throw new ArgumentNullException(nameof(indexModels));
            }

            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var clients = provider.GetRequiredService<IDictionary<string, IMongoClient>>();
            if (!clients.ContainsKey(name))
            {
                throw new ArgumentException(ClientNotRegistered(name), nameof(name));
            }

            var client = clients[name];
            var monitor = provider.GetRequiredService<IOptionsMonitor<MongoDataOptions>>();
            var options = monitor.Get(name);
            var type = typeof(TDocument);
            if (!options.CollectionNames.TryGetValue(type.Name, out var collectionName))
            {
                throw new ArgumentException(CollectionNameNotFound(type.Name));
            }

            async Task<IServiceProvider> BuildIndexes()
            {
                await client
                    .GetDatabase(options.DatabaseName, options.DatabaseSettings)
                    .GetCollection<TDocument>(collectionName, options.MongoCollectionSettings)
                    .Indexes.CreateManyAsync(indexModels, options.CreateManyIndexesOptions, cancellationToken).ConfigureAwait(false);
                return provider;
            }

            return BuildIndexes();
        }

        /// <summary>If the <see cref="IMongoCollection{TDocument}"/> is empty, inserts the <paramref name="documents"/> using the <see cref="IMongoClient"/> identified by <paramref name="name"/>.</summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> instance.</param>
        /// <param name="documents">The documents to insert.</param>
        /// <param name="name">The name of the <see cref="MongoDataOptions"/> instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="TDocument">The type of the <paramref name="documents"/>.</typeparam>
        /// <returns>The <paramref name="provider"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> is <see langword="null" /> or <paramref name="documents"/> is <see langword="null" /> or <paramref name="name"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">No <see cref="IMongoClient"/> has been added to the <paramref name="provider"/>
        /// -or-
        /// collection name not found for <typeparamref name="TDocument"/>.</exception>
        public static Task<IServiceProvider> SeedDocumentsAsync<TDocument>(
            this IServiceProvider provider,
            IEnumerable<TDocument> documents,
            string name = nameof(MongoDataService),
            CancellationToken cancellationToken = default)
            where TDocument : class
        {
            if (provider == default)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (documents == default)
            {
                throw new ArgumentNullException(nameof(documents));
            }

            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var clients = provider.GetRequiredService<IDictionary<string, IMongoClient>>();
            if (!clients.ContainsKey(name))
            {
                throw new ArgumentException(ClientNotRegistered(name), nameof(name));
            }

            var client = clients[name];
            var monitor = provider.GetRequiredService<IOptionsMonitor<MongoDataOptions>>();
            var options = monitor.Get(name);
            var database = client.GetDatabase(options.DatabaseName, options.DatabaseSettings);
            var type = typeof(TDocument);
            if (!options.CollectionNames.TryGetValue(type.Name, out var collectionName))
            {
                throw new ArgumentException(CollectionNameNotFound(type.Name));
            }

            async Task<IServiceProvider> SeedDataAsync()
            {
                var collection = database.GetCollection<TDocument>(collectionName, options.MongoCollectionSettings);
                var count = await collection.CountDocumentsAsync(FilterDefinition<TDocument>.Empty, options.CountOptions, cancellationToken).ConfigureAwait(false);
                if (count <= 0)
                {
                    await collection.InsertManyAsync(documents, options.InsertManyOptions, cancellationToken).ConfigureAwait(false);
                }

                return provider;
            }

            return SeedDataAsync();
        }

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
