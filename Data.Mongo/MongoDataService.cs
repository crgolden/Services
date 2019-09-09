namespace Services
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using MongoDB.Driver;

    public class MongoDataService : IDataService
    {
        private readonly IMongoDatabase _database;

        public MongoDataService(IMongoDatabase database) => _database = database;

        public async Task<T> CreateAsync<T>(
            T document,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (document.Equals(default))
            {
                throw new ArgumentNullException(nameof(document), "Invalid Document");
            }

            var collection = _database.GetCollection<T>(typeof(T).Name);
            await collection.InsertOneAsync(document, default, cancellationToken).ConfigureAwait(false);
            return document;
        }

        public async Task<T> ReadAsync<T>(
            Expression<Func<T, bool>> filter,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (filter.Equals(default))
            {
                throw new ArgumentNullException(nameof(filter), "Invalid Filter");
            }

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
            if (filter.Equals(default))
            {
                throw new ArgumentNullException(nameof(filter), "Invalid Filter");
            }

            if (document.Equals(default))
            {
                throw new ArgumentNullException(nameof(document), "Invalid Document");
            }

            var collection = _database.GetCollection<T>(typeof(T).Name);
            await collection.ReplaceOneAsync(filter, document, default, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAsync<T>(
            Expression<Func<T, bool>> filter,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (filter.Equals(default))
            {
                throw new ArgumentNullException(nameof(filter), "Invalid Filter");
            }

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
