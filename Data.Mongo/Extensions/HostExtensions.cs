namespace Services.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using Services;
    using static System.String;
    using static System.Threading.Tasks.Task;
    using static MongoDB.Bson.Serialization.BsonClassMap;
    using static Constants.ExceptionMessages;

    /// <summary>A class with methods that extend <see cref="IHost"/>.</summary>
    [PublicAPI]
    public static class HostExtensions
    {
        /// <summary>Registers all <paramref name="keyValuePairs"/> values and adds all <paramref name="keyValuePairs"/> keys to the <see cref="MongoDataOptions"/> instance identified by <paramref name="name"/>.</summary>
        /// <param name="host">The <see cref="IHost"/> instance.</param>
        /// <param name="keyValuePairs">The collection name/class map pairs.</param>
        /// <param name="name">The name of the <see cref="MongoDataOptions"/> instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <paramref name="host"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="host"/> is <see langword="null" /> or <paramref name="keyValuePairs"/> is <see langword="null" /> or <paramref name="name"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">No <see cref="IMongoClient"/> has been added to the <paramref name="host"/> container.</exception>
        public static Task<IHost> AddMongo(
            this IHost host,
            IEnumerable<KeyValuePair<string, BsonClassMap>> keyValuePairs,
            string name = nameof(MongoDB),
            CancellationToken cancellationToken = default)
        {
            if (host == default)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (keyValuePairs == null)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }

            if (IsNullOrEmpty(name) || IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var clients = host.Services.GetRequiredService<IDictionary<string, IMongoClient>>();
            if (!clients.ContainsKey(name))
            {
                throw new ArgumentException(ClientNotRegistered(name), nameof(name));
            }

            var monitor = host.Services.GetRequiredService<IOptionsMonitor<MongoDataOptions>>();
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

            async Task<IHost> InitializeCollectionsAsync()
            {
                var cursor = await database.ListCollectionNamesAsync(options.ListCollectionNamesOptions, cancellationToken).ConfigureAwait(false);
                var collectionNames = await cursor.ToListAsync(cancellationToken).ConfigureAwait(false);
                var tasks = options.CollectionNames.Values
                    .Where(x => !IsNullOrEmpty(x) && !IsNullOrWhiteSpace(x) && collectionNames.All(y => y != x))
                    .Select(x => database.CreateCollectionAsync(x, options.CreateCollectionOptions, cancellationToken));
                await WhenAll(tasks).ConfigureAwait(false);
                return host;
            }

            return InitializeCollectionsAsync();
        }

        /// <summary>If the <see cref="IMongoCollection{TDocument}"/> is empty, inserts the <paramref name="documents"/> using the <see cref="IMongoClient"/> identified by <paramref name="name"/>.</summary>
        /// <param name="host">The <see cref="IHost"/> instance.</param>
        /// <param name="documents">The documents to insert.</param>
        /// <param name="name">The name of the <see cref="MongoDataOptions"/> instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="TDocument">The type of the <paramref name="documents"/>.</typeparam>
        /// <returns>The <paramref name="host"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="host"/> is <see langword="null" /> or <paramref name="documents"/> is <see langword="null" /> or <paramref name="name"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">No <see cref="IMongoClient"/> has been added to the <paramref name="host"/> container
        /// -or-
        /// collection name not found for <typeparamref name="TDocument"/>.</exception>
        public static Task<IHost> SeedDocumentsAsync<TDocument>(
            this IHost host,
            IEnumerable<TDocument> documents,
            string name = nameof(MongoDB),
            CancellationToken cancellationToken = default)
            where TDocument : class
        {
            if (host == default)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (documents == null)
            {
                throw new ArgumentNullException(nameof(documents));
            }

            if (IsNullOrEmpty(name) || IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var clients = host.Services.GetRequiredService<IDictionary<string, IMongoClient>>();
            if (!clients.ContainsKey(name))
            {
                throw new ArgumentException(ClientNotRegistered(name), nameof(name));
            }

            var client = clients[name];
            var monitor = host.Services.GetRequiredService<IOptionsMonitor<MongoDataOptions>>();
            var options = monitor.Get(name);
            var database = client.GetDatabase(options.DatabaseName, options.DatabaseSettings);
            var type = typeof(TDocument);
            if (!options.CollectionNames.TryGetValue(type.Name, out var collectionName))
            {
                throw new ArgumentException(CollectionNameNotFound(type.Name));
            }

            var collection = database.GetCollection<TDocument>(collectionName, options.MongoCollectionSettings);

            async Task<IHost> SeedDataAsync()
            {
                var count = await collection.CountDocumentsAsync(FilterDefinition<TDocument>.Empty, options.CountOptions, cancellationToken).ConfigureAwait(false);
                if (count <= 0)
                {
                    await collection.InsertManyAsync(documents, options.InsertManyOptions, cancellationToken).ConfigureAwait(false);
                }

                return host;
            }

            return SeedDataAsync();
        }
    }
}
