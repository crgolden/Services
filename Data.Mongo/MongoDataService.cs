namespace Services
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;

    public class MongoDataService : IDataService
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDataService> _logger;

        public MongoDataService(
            IMongoDatabase database,
            ILogger<MongoDataService> logger)
        {
            _database = database;
            _logger = logger;
        }

        public async Task<T> CreateAsync<T>(
            T document,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            await collection.InsertOneAsync(document, default, cancellationToken).ConfigureAwait(false);
            return document;
        }

        public async Task<T> ReadAsync<T>(
            Expression<Func<T, bool>> filter,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var cursor = await collection.FindAsync(filter, default, cancellationToken).ConfigureAwait(false);
            return await cursor.SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateAsync<T>(
            Expression<Func<T, bool>> filter,
            T document,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            await collection.ReplaceOneAsync(filter, document, default, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAsync<T>(
            Expression<Func<T, bool>> filter,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            await collection.DeleteOneAsync(filter, default, cancellationToken).ConfigureAwait(false);
        }

        public IQueryable<T> List<T>()
            where T : class
        {
            return _database.GetCollection<T>(typeof(T).Name).AsQueryable();
        }
    }
}
