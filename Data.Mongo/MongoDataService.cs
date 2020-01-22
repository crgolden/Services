namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using JetBrains.Annotations;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using static System.GC;
    using static System.String;
    using static System.Threading.CancellationToken;
    using static System.Threading.Tasks.Task;
    using static Constants.ExceptionMessages;
    using static MongoDB.Bson.Serialization.BsonClassMap;

    /// <inheritdoc cref="IDataService" />
    [PublicAPI]
    public class MongoDataService : IDataService, IDisposable
    {
        private bool _disposedValue;

        /// <summary>Initializes a new instance of the <see cref="MongoDataService"/> class.</summary>
        /// <param name="mongoClient">The <see cref="IMongoClient"/>.</param>
        /// <param name="mongoDataOptions">The <see cref="MongoDataOptions"/>.</param>
        /// <param name="name">The name (default is "MongoDB").</param>
        /// <exception cref="ArgumentNullException"><paramref name="mongoClient"/> is <see langword="null" /> or <paramref name="mongoDataOptions"/> is <see langword="null" /> or <paramref name="name"/> is <see langword="null" />.</exception>
        public MongoDataService(IMongoClient mongoClient, MongoDataOptions mongoDataOptions, string name = nameof(MongoDB))
        {
            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Options = mongoDataOptions ?? throw new ArgumentNullException(nameof(mongoDataOptions));
            Client = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
            Database = mongoClient.GetDatabase(Options.DatabaseName, Options.DatabaseSettings);
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>Gets the <see cref="MongoDataOptions"/>.</summary>
        /// <value>The <see cref="MongoDataOptions"/>.</value>
        protected MongoDataOptions Options { get; }

        /// <summary>Gets the <see cref="IMongoClient"/>.</summary>
        /// <value>The <see cref="IMongoClient"/>.</value>
        protected IMongoClient Client { get; }

        /// <summary>Gets or sets the <see cref="IClientSessionHandle"/>.</summary>
        /// <value>The <see cref="IClientSessionHandle"/>.</value>
        protected IClientSessionHandle Session { get; set; }

        /// <summary> Gets the <see cref="IMongoDatabase"/>.</summary>
        /// <value>The <see cref="IMongoDatabase"/>.</value>
        protected IMongoDatabase Database { get; }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="record"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Collection name not found for <typeparamref name="T"/>.</exception>
        public virtual Task<T> CreateAsync<T>(T record, CancellationToken cancellationToken = default)
            where T : class
        {
            if (record == default)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var type = typeof(T);
            if (!Options.CollectionNames.TryGetValue(type.Name, out var collectionName))
            {
                throw new ArgumentException(CollectionNameNotFound(type.Name));
            }

            var collection = Database.GetCollection<T>(collectionName);
            if (!Options.UseClientSession)
            {
                async Task<T> CreateAsync()
                {
                    await collection.InsertOneAsync(record, Options.InsertOneOptions, cancellationToken).ConfigureAwait(false);
                    return record;
                }

                return CreateAsync();
            }
            else
            {
                async Task<T> CreateAsync()
                {
                    if (Session == default)
                    {
                        Session = await Client.StartSessionAsync(Options.ClientSessionOptions, cancellationToken).ConfigureAwait(false);
                    }

                    if (!Session.IsInTransaction)
                    {
                        Session.StartTransaction(Options.TransactionOptions);
                    }

                    await collection.InsertOneAsync(Session, record, Options.InsertOneOptions, cancellationToken).ConfigureAwait(false);
                    return record;
                }

                return CreateAsync();
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="records"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Collection name not found for <typeparamref name="T"/>.</exception>
        public virtual Task<IEnumerable<T>> CreateRangeAsync<T>(IEnumerable<T> records, CancellationToken cancellationToken = default)
            where T : class
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            var type = typeof(T);
            if (!Options.CollectionNames.TryGetValue(type.Name, out var collectionName))
            {
                throw new ArgumentException(CollectionNameNotFound(type.Name));
            }

            var enumerated = records.ToArray();
            var collection = Database.GetCollection<T>(collectionName);
            if (!Options.UseClientSession)
            {
                async Task<IEnumerable<T>> CreateRangeAsync()
                {
                    await collection.InsertManyAsync(enumerated, Options.InsertManyOptions, cancellationToken).ConfigureAwait(false);
                    return enumerated;
                }

                return CreateRangeAsync();
            }
            else
            {
                async Task<IEnumerable<T>> CreateRangeAsync()
                {
                    if (Session == default)
                    {
                        Session = await Client.StartSessionAsync(Options.ClientSessionOptions, cancellationToken).ConfigureAwait(false);
                    }

                    if (!Session.IsInTransaction)
                    {
                        Session.StartTransaction(Options.TransactionOptions);
                    }

                    await collection.InsertManyAsync(Session, enumerated, Options.InsertManyOptions, cancellationToken).ConfigureAwait(false);
                    return enumerated;
                }

                return CreateRangeAsync();
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Collection name not found for <typeparamref name="T"/>.</exception>
        public virtual Task DeleteAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where T : class
        {
            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var type = typeof(T);
            if (!Options.CollectionNames.TryGetValue(type.Name, out var collectionName))
            {
                throw new ArgumentException(CollectionNameNotFound(type.Name));
            }

            var collection = Database.GetCollection<T>(collectionName, Options.MongoCollectionSettings);
            if (!Options.UseClientSession)
            {
                return collection.DeleteOneAsync(predicate, Options.DeleteOptions, cancellationToken);
            }

            async Task DeleteAsync()
            {
                if (Session == default)
                {
                    Session = await Client.StartSessionAsync(Options.ClientSessionOptions, cancellationToken).ConfigureAwait(false);
                }

                if (!Session.IsInTransaction)
                {
                    Session.StartTransaction(Options.TransactionOptions);
                }

                await collection.DeleteOneAsync(Session, predicate, Options.DeleteOptions, cancellationToken).ConfigureAwait(false);
            }

            return DeleteAsync();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="keyValuePairs"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Collection name not found for <typeparamref name="T"/>.</exception>
        public virtual Task DeleteRangeAsync<T>(IDictionary<Expression<Func<T, bool>>, T> keyValuePairs, CancellationToken cancellationToken = default)
            where T : class
        {
            if (keyValuePairs == default)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }

            var type = typeof(T);
            if (!Options.CollectionNames.TryGetValue(type.Name, out var collectionName))
            {
                throw new ArgumentException(CollectionNameNotFound(type.Name));
            }

            var collection = Database.GetCollection<T>(collectionName, Options.MongoCollectionSettings);
            if (!Options.UseClientSession)
            {
                IEnumerable<WriteModel<T>> requests = from keyValuePair in keyValuePairs
                    where keyValuePair.Key != default
                    select new DeleteOneModel<T>(keyValuePair.Key);
                return collection.BulkWriteAsync(requests, Options.BulkWriteOptions, cancellationToken);
            }

            async Task DeleteRangeAsync()
            {
                if (Session == default)
                {
                    Session = await Client.StartSessionAsync(Options.ClientSessionOptions, cancellationToken).ConfigureAwait(false);
                }

                if (!Session.IsInTransaction)
                {
                    Session.StartTransaction(Options.TransactionOptions);
                }

                IEnumerable<WriteModel<T>> requests = from keyValuePair in keyValuePairs
                    where keyValuePair.Key != default
                    select new DeleteOneModel<T>(keyValuePair.Key);
                await collection.BulkWriteAsync(Session, requests, Options.BulkWriteOptions, cancellationToken).ConfigureAwait(false);
            }

            return DeleteRangeAsync();
        }

        /// <inheritdoc />
        public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (!Options.UseClientSession || !Session.IsInTransaction)
            {
                return;
            }

            try
            {
                await Session.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await Session.AbortTransactionAsync(None).ConfigureAwait(false);
                throw;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">Collection name not found for <typeparamref name="T"/>.</exception>
        public virtual Task UpdateAsync<T>(Expression<Func<T, bool>> predicate, T record, CancellationToken cancellationToken = default)
            where T : class
        {
            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (record == default)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var type = typeof(T);
            if (!Options.CollectionNames.TryGetValue(type.Name, out var collectionName))
            {
                throw new ArgumentException(CollectionNameNotFound(type.Name));
            }

            var collection = Database.GetCollection<T>(collectionName, Options.MongoCollectionSettings);
            if (!Options.UseClientSession)
            {
                return collection.ReplaceOneAsync(predicate, record, Options.ReplaceOptions, cancellationToken);
            }

            async Task UpdateAsync()
            {
                if (Session == default)
                {
                    Session = await Client.StartSessionAsync(Options.ClientSessionOptions, cancellationToken).ConfigureAwait(false);
                }

                if (!Session.IsInTransaction)
                {
                    Session.StartTransaction(Options.TransactionOptions);
                }

                await collection.ReplaceOneAsync(Session, predicate, record, Options.ReplaceOptions, cancellationToken).ConfigureAwait(false);
            }

            return UpdateAsync();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="keyValuePairs"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Collection name not found for <typeparamref name="T"/>.</exception>
        public virtual Task UpdateRangeAsync<T>(IDictionary<Expression<Func<T, bool>>, T> keyValuePairs, CancellationToken cancellationToken = default)
            where T : class
        {
            if (keyValuePairs == default)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }

            var type = typeof(T);
            if (!Options.CollectionNames.TryGetValue(type.Name, out var collectionName))
            {
                throw new ArgumentException(CollectionNameNotFound(type.Name));
            }

            var collection = Database.GetCollection<T>(collectionName, Options.MongoCollectionSettings);
            if (!Options.UseClientSession)
            {
                IEnumerable<WriteModel<T>> requests = from keyValuePair in keyValuePairs
                    where keyValuePair.Key != default && keyValuePair.Value != null
                    select new ReplaceOneModel<T>(keyValuePair.Key, keyValuePair.Value);
                return collection.BulkWriteAsync(requests, Options.BulkWriteOptions, cancellationToken);
            }

            async Task UpdateRangeAsync()
            {
                if (Session == default)
                {
                    Session = await Client.StartSessionAsync(Options.ClientSessionOptions, cancellationToken).ConfigureAwait(false);
                }

                if (!Session.IsInTransaction)
                {
                    Session.StartTransaction(Options.TransactionOptions);
                }

                IEnumerable<WriteModel<T>> requests = from keyValuePair in keyValuePairs
                    where keyValuePair.Key != default && keyValuePair.Value != null
                    select new ReplaceOneModel<T>(keyValuePair.Key, keyValuePair.Value);
                await collection.BulkWriteAsync(Session, requests, Options.BulkWriteOptions, cancellationToken).ConfigureAwait(false);
            }

            return UpdateRangeAsync();
        }

        /// <inheritdoc />
        public virtual Task<bool> AnyAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.AnyAsync(cancellationToken)
                : FromResult(source.Any());
        }

        /// <inheritdoc />
        public virtual Task<bool> AnyAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.AnyAsync(predicate, cancellationToken)
                : FromResult(source.Any(predicate));
        }

        /// <inheritdoc />
        public virtual Task<decimal?> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, decimal?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.AverageAsync(selector, cancellationToken)
                : FromResult(source.Average(selector));
        }

        /// <inheritdoc />
        public virtual Task<decimal> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.AverageAsync(selector, cancellationToken)
                : FromResult(source.Average(selector));
        }

        /// <inheritdoc />
        public virtual Task<double?> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, double?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.AverageAsync(selector, cancellationToken)
                : FromResult(source.Average(selector));
        }

        /// <inheritdoc />
        public virtual Task<double> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, double>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.AverageAsync(selector, cancellationToken)
                : FromResult(source.Average(selector));
        }

        /// <inheritdoc />
        public virtual Task<float?> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, float?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.AverageAsync(selector, cancellationToken)
                : FromResult(source.Average(selector));
        }

        /// <inheritdoc />
        public virtual Task<float> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, float>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.AverageAsync(selector, cancellationToken)
                : FromResult(source.Average(selector));
        }

        /// <inheritdoc />
        public virtual Task<double?> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, int?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.AverageAsync(selector, cancellationToken)
                : FromResult(source.Average(selector));
        }

        /// <inheritdoc />
        public virtual Task<double> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, int>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.AverageAsync(selector, cancellationToken)
                : FromResult(source.Average(selector));
        }

        /// <inheritdoc />
        public virtual Task<double?> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, long?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.AverageAsync(selector, cancellationToken)
                : FromResult(source.Average(selector));
        }

        /// <inheritdoc />
        public virtual Task<double> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, long>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.AverageAsync(selector, cancellationToken)
                : FromResult(source.Average(selector));
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.CountAsync(cancellationToken)
                : FromResult(source.Count());
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.CountAsync(predicate, cancellationToken)
                : FromResult(source.Count(predicate));
        }

        /// <inheritdoc />
        public virtual Task<T> FirstAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.FirstAsync(cancellationToken)
                : FromResult(source.First());
        }

        /// <inheritdoc />
        public virtual Task<T> FirstAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.FirstAsync(predicate, cancellationToken)
                : FromResult(source.First(predicate));
        }

        /// <inheritdoc />
        public virtual Task<T> FirstOrDefaultAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.FirstOrDefaultAsync(cancellationToken)
                : FromResult(source.FirstOrDefault());
        }

        /// <inheritdoc />
        public virtual Task<T> FirstOrDefaultAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.FirstOrDefaultAsync(predicate, cancellationToken)
                : FromResult(source.FirstOrDefault(predicate));
        }

        /// <inheritdoc />
        public virtual Task ForEachAsync<T>(IQueryable<T> source, Action<T> action, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == default)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (source is IMongoQueryable<T> mongoQuery)
            {
                return mongoQuery.ForEachAsync(action, cancellationToken);
            }

            source.ToList().ForEach(action);
            return CompletedTask;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">
        /// <paramref name="keyValues"/> does not contain exactly one key value or
        /// -or-
        /// collection name not found for <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Class map not found for <typeparamref name="T"/>
        /// -or-
        /// id member not found for <typeparamref name="T"/>.
        /// </exception>
        public ValueTask<T> GetAsync<T>(IQueryable<T> source, object[] keyValues, CancellationToken cancellationToken = default)
            where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keyValues == default)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            if (keyValues.Length != 1)
            {
                throw new ArgumentException("Key values must contain exactly one key value", nameof(keyValues));
            }

            var type = typeof(T);
            if (!Options.CollectionNames.TryGetValue(type.Name, out var collectionName))
            {
                throw new ArgumentException(CollectionNameNotFound(type.Name));
            }

            var classMap = LookupClassMap(type);
            if (classMap == default)
            {
                throw new InvalidOperationException($"Class map not found for '{type.Name}'");
            }

            var id = classMap.IdMemberMap;
            if (id == default)
            {
                throw new InvalidOperationException($"Id member not found for '{type.Name}'");
            }

            var filter = Builders<T>.Filter.Eq(id.ElementName, keyValues[0]);
            var collection = Database.GetCollection<T>(collectionName, Options.MongoCollectionSettings);

            async ValueTask<T> GetAsync()
            {
                var cursor = await collection.FindAsync<T>(filter, default, cancellationToken).ConfigureAwait(false);
                return await cursor.SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            return GetAsync();
        }

        /// <inheritdoc />
        public virtual Task<long> LongCountAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.LongCountAsync(cancellationToken)
                : FromResult(source.LongCount());
        }

        /// <inheritdoc />
        public virtual Task<long> LongCountAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.LongCountAsync(predicate, cancellationToken)
                : FromResult(source.LongCount(predicate));
        }

        /// <inheritdoc />
        public virtual Task<T> MaxAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.MaxAsync(cancellationToken)
                : FromResult(source.Max());
        }

        /// <inheritdoc />
        public virtual Task<TResult> MaxAsync<T, TResult>(IQueryable<T> source, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.MaxAsync(selector, cancellationToken)
                : FromResult(source.Max(selector));
        }

        /// <inheritdoc />
        public virtual Task<T> MinAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.MinAsync(cancellationToken)
                : FromResult(source.Min());
        }

        /// <inheritdoc />
        public virtual Task<TResult> MinAsync<T, TResult>(IQueryable<T> source, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.MinAsync(selector, cancellationToken)
                : FromResult(source.Min(selector));
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">Collection name not found for <typeparamref name="T"/>.</exception>
        public virtual IQueryable<T> Query<T>()
            where T : class
        {
            var type = typeof(T);
            if (!Options.CollectionNames.TryGetValue(type.Name, out var collectionName))
            {
                throw new ArgumentException(CollectionNameNotFound(type.Name));
            }

            var collection = Database.GetCollection<T>(collectionName);
            return collection.AsQueryable(Options.AggregateOptions);
        }

        /// <inheritdoc />
        public virtual Task<T> SingleAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.SingleAsync(cancellationToken)
                : FromResult(source.Single());
        }

        /// <inheritdoc />
        public virtual Task<T> SingleAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.SingleAsync(predicate, cancellationToken)
                : FromResult(source.Single(predicate));
        }

        /// <inheritdoc />
        public virtual Task<T> SingleOrDefaultAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.SingleOrDefaultAsync(cancellationToken)
                : FromResult(source.SingleOrDefault());
        }

        /// <inheritdoc />
        public virtual Task<T> SingleOrDefaultAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.SingleOrDefaultAsync(predicate, cancellationToken)
                : FromResult(source.SingleOrDefault(predicate));
        }

        /// <inheritdoc />
        public virtual Task<decimal?> SumAsync<T>(IQueryable<T> source, Expression<Func<T, decimal?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.SumAsync(selector, cancellationToken)
                : FromResult(source.Sum(selector));
        }

        /// <inheritdoc />
        public virtual Task<decimal> SumAsync<T>(IQueryable<T> source, Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.SumAsync(selector, cancellationToken)
                : FromResult(source.Sum(selector));
        }

        /// <inheritdoc />
        public virtual Task<double?> SumAsync<T>(IQueryable<T> source, Expression<Func<T, double?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.SumAsync(selector, cancellationToken)
                : FromResult(source.Sum(selector));
        }

        /// <inheritdoc />
        public virtual Task<double> SumAsync<T>(IQueryable<T> source, Expression<Func<T, double>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.SumAsync(selector, cancellationToken)
                : FromResult(source.Sum(selector));
        }

        /// <inheritdoc />
        public virtual Task<float?> SumAsync<T>(IQueryable<T> source, Expression<Func<T, float?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.SumAsync(selector, cancellationToken)
                : FromResult(source.Sum(selector));
        }

        /// <inheritdoc />
        public virtual Task<float> SumAsync<T>(IQueryable<T> source, Expression<Func<T, float>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.SumAsync(selector, cancellationToken)
                : FromResult(source.Sum(selector));
        }

        /// <inheritdoc />
        public virtual Task<int?> SumAsync<T>(IQueryable<T> source, Expression<Func<T, int?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.SumAsync(selector, cancellationToken)
                : FromResult(source.Sum(selector));
        }

        /// <inheritdoc />
        public virtual Task<int> SumAsync<T>(IQueryable<T> source, Expression<Func<T, int>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.SumAsync(selector, cancellationToken)
                : FromResult(source.Sum(selector));
        }

        /// <inheritdoc />
        public virtual Task<long?> SumAsync<T>(IQueryable<T> source, Expression<Func<T, long?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.SumAsync(selector, cancellationToken)
                : FromResult(source.Sum(selector));
        }

        /// <inheritdoc />
        public virtual Task<long> SumAsync<T>(IQueryable<T> source, Expression<Func<T, long>> selector, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == default)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.SumAsync(selector, cancellationToken)
                : FromResult(source.Sum(selector));
        }

        /// <inheritdoc />
        public virtual Task<List<T>> ToListAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source is IMongoQueryable<T> mongoQuery
                ? mongoQuery.ToListAsync(cancellationToken)
                : FromResult(source.ToList());
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            SuppressFinalize(this);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <param name="disposing">Indicates whether the method call comes from a Dispose method (its value is <see langword="true" />) or from a finalizer (its value is <see langword="false" />).</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                Session?.Dispose();
            }

            _disposedValue = true;
        }
    }
}
