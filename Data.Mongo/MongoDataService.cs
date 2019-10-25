namespace Services
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;
    using static System.DateTime;
    using static Common.DataServiceType;
    using static Common.EventId;
    using static Microsoft.Extensions.Logging.LogLevel;
    using EventId = Microsoft.Extensions.Logging.EventId;

    /// <inheritdoc />
    public class MongoDataService : IDataService
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDataService> _logger;
        private readonly MongoDataOptions _mongoDataOptions;

        public MongoDataService(
            IMongoDatabase? database,
            ILogger<MongoDataService>? logger,
            IOptions<MongoDataOptions> mongoDataOptions)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mongoDataOptions = mongoDataOptions?.Value ?? throw new ArgumentNullException(nameof(mongoDataOptions));
        }

        /// <inheritdoc />
        public string? Name { get; set; }

        /// <inheritdoc />
        public DataServiceType Type => Mongo;

        /// <inheritdoc />
        public Task<T> CreateAsync<T>(
            T? record,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var collectionName = GetCollectionName<T>();
            if (record == default)
            {
                throw new ArgumentNullException(nameof(record));
            }

            return Create(record, collectionName, logLevel, cancellationToken);
        }

        /// <inheritdoc />
        public Task<T?> ReadAsync<T>(
            Expression<Func<T, bool>>? expression,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var collectionName = GetCollectionName<T>();
            if (expression == default)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return Read(expression, collectionName, logLevel, cancellationToken);
        }

        /// <inheritdoc />
        public Task UpdateAsync<T>(
            Expression<Func<T, bool>>? expression,
            T? record,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var collectionName = GetCollectionName<T>();
            if (expression == default)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (record == default)
            {
                throw new ArgumentNullException(nameof(record));
            }

            return Update(expression, record, collectionName, logLevel, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteAsync<T>(
            Expression<Func<T, bool>>? expression,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var collectionName = GetCollectionName<T>();
            if (expression == default)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return Delete(expression, collectionName, logLevel, cancellationToken);
        }

        /// <inheritdoc />
        public IQueryable<T> List<T>(LogLevel logLevel = Information)
            where T : class
        {
            var typeName = typeof(T).Name;
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataListStart, $"{DataListStart}"),
                    message: "Listing type {@Type} at {@Time}",
                    args: new object[] { typeName, UtcNow });
                var collectionName = GetCollectionName<T>();
                var queryable = _database.GetCollection<T>(collectionName).AsQueryable();
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataListEnd, $"{DataListEnd}"),
                    message: "Listed type {@Type} at {@Time}",
                    args: new object[] { typeName, UtcNow });
                return queryable;
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataListError, $"{DataListError}"),
                    exception: e,
                    message: "Error listing type {@Type} at {@Time}",
                    args: new object[] { typeName, UtcNow });
                throw;
            }
        }

        private string GetCollectionName<T>()
        {
            var typeName = typeof(T).Name;
            if (!_mongoDataOptions.CollectionNames.TryGetValue(typeName, out var name))
            {
                throw new ArgumentException($"Collection name not found for type '{typeName}'");
            }

            return name;
        }

        private async Task<T> Create<T>(
            T record,
            string collectionName,
            LogLevel logLevel,
            CancellationToken cancellationToken)
            where T : class
        {
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataCreateStart, $"{DataCreateStart}"),
                    message: "Creating document {@Document} at {@Time}",
                    args: new object[] { record, UtcNow });
                var collection = _database.GetCollection<T>(collectionName);
                await collection.InsertOneAsync(record, default, cancellationToken).ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataCreateEnd, $"{DataCreateEnd}"),
                    message: "Created document {@Document} at {@Time}",
                    args: new object[] { record, UtcNow });
                return record;
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataCreateError, $"{DataCreateError}"),
                    exception: e,
                    message: "Error creating document {@Document} at {@Time}",
                    args: new object[] { record, UtcNow });
                throw;
            }
        }

        private async Task<T?> Read<T>(
            Expression<Func<T, bool>> expression,
            string collectionName,
            LogLevel logLevel,
            CancellationToken cancellationToken)
            where T : class
        {
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataReadStart, $"{DataReadStart}"),
                    message: "Reading filter {@Filter} at {@Time}",
                    args: new object[] { expression.Body, UtcNow });
                var collection = _database.GetCollection<T>(collectionName);
                var cursor = await collection.FindAsync(expression, default, cancellationToken).ConfigureAwait(false);
                var document = await cursor.SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataReadEnd, $"{DataReadEnd}"),
                    message: "Read document {@Document} with filter {@Filter} at {@Time}",
                    args: new object[] { document, expression.Body, UtcNow });
                return document;
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataReadError, $"{DataReadError}"),
                    exception: e,
                    message: "Error reading filter {@Filter} at {@Time}",
                    args: new object[] { expression.Body, UtcNow });
                throw;
            }
        }

        private async Task Update<T>(
            Expression<Func<T, bool>> expression,
            T record,
            string collectionName,
            LogLevel logLevel,
            CancellationToken cancellationToken)
            where T : class
        {
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataUpdateStart, $"{DataUpdateStart}"),
                    message: "Updating document {@Document} with filter {@Filter} at {@Time}",
                    args: new object[] { record, expression.Body, UtcNow });
                var collection = _database.GetCollection<T>(collectionName);
                await collection.ReplaceOneAsync(expression, record, default, cancellationToken).ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataUpdateEnd, $"{DataUpdateEnd}"),
                    message: "Updated document {@Document} with filter {@Filter} at {@Time}",
                    args: new object[] { record, expression.Body, UtcNow });
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataUpdateError, $"{DataUpdateError}"),
                    exception: e,
                    message: "Error updating document {@Document} with filter {@Filter} at {@Time}",
                    args: new object[] { record, expression.Body, UtcNow });
                throw;
            }
        }

        private async Task Delete<T>(
            Expression<Func<T, bool>> expression,
            string collectionName,
            LogLevel logLevel,
            CancellationToken cancellationToken)
            where T : class
        {
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataDeleteStart, $"{DataDeleteStart}"),
                    message: "Deleting with filter {@Filter} at {@Time}",
                    args: new object[] { expression.Body, UtcNow });
                var collection = _database.GetCollection<T>(collectionName);
                await collection.DeleteOneAsync(expression, default, cancellationToken).ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataDeleteEnd, $"{DataDeleteEnd}"),
                    message: "Deleted with filter {@Filter} at {@Time}",
                    args: new object[] { expression.Body, UtcNow });
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataDeleteError, $"{DataDeleteError}"),
                    exception: e,
                    message: "Error deleting with filter {@Filter} at {@Time}",
                    args: new object[] { expression.Body, UtcNow });
                throw;
            }
        }
    }
}
