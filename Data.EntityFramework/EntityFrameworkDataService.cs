namespace Services
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using static System.DateTime;
    using static Common.DataServiceType;
    using static Common.EventId;
    using static Microsoft.EntityFrameworkCore.EntityState;
    using static Microsoft.Extensions.Logging.LogLevel;
    using EventId = Microsoft.Extensions.Logging.EventId;

    /// <inheritdoc />
    public class EntityFrameworkDataService : IDataService
    {
        private readonly DbContext _context;
        private readonly ILogger<EntityFrameworkDataService> _logger;

        public EntityFrameworkDataService(
            DbContext? context,
            ILogger<EntityFrameworkDataService>? logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public string? Name { get; set; }

        /// <inheritdoc />
        public DataServiceType Type => EntityFramework;

        /// <inheritdoc />
        public async Task<T> CreateAsync<T>(
            T? entity,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataCreateStart, $"{DataCreateStart}"),
                    message: "Creating entity {@Entity} at {@Time}",
                    args: new object[] { entity, UtcNow });
                _context.Entry(entity).State = Added;
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataCreateEnd, $"{DataCreateEnd}"),
                    message: "Created entity {@Entity} at {@Time}",
                    args: new object[] { entity, UtcNow });
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    eventId: new EventId((int)DataCreateError, $"{DataCreateError}"),
                    exception: e,
                    message: "Error creating entity {@Entity} at {@Time}",
                    args: new object[] { entity, UtcNow });
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<T?> ReadAsync<T>(
            Expression<Func<T, bool>>? predicate,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataReadStart, $"{DataReadStart}"),
                    message: "Reading predicate {@Predicate} at {@Time}",
                    args: new object[] { predicate.Body, UtcNow });
                var entity = await _context.Set<T>().SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataReadEnd, $"{DataReadEnd}"),
                    message: "Read entity {@Entity} with predicate {@Predicate} at {@Time}",
                    args: new object[] { entity, predicate.Body, UtcNow });
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    eventId: new EventId((int)DataReadError, $"{DataReadError}"),
                    exception: e,
                    message: "Error reading predicate {@Predicate} at {@Time}",
                    args: new object[] { predicate.Body, UtcNow });
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync<T>(
            Expression<Func<T, bool>>? predicate,
            T? entity,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataUpdateStart, $"{DataUpdateStart}"),
                    message: "Updating entity {@Entity} at {@Time}",
                    args: new object[] { entity, UtcNow });
                _context.Entry(entity).State = Modified;
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataUpdateEnd, $"{DataUpdateEnd}"),
                    message: "Updated entity {@Entity} at {@Time}",
                    args: new object[] { entity, UtcNow });
            }
            catch (Exception e)
            {
                _logger.LogError(
                    eventId: new EventId((int)DataUpdateError, $"{DataUpdateError}"),
                    exception: e,
                    message: "Error updating entity {@Entity} at {@Time}",
                    args: new object[] { entity, UtcNow });
                throw;
            }
        }

        /// <inheritdoc />
        public async Task DeleteAsync<T>(
            Expression<Func<T, bool>>? predicate,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataDeleteStart, $"{DataDeleteStart}"),
                    message: "Deleting with predicate {@Predicate} at {@Time}",
                    args: new object[] { predicate.Body, UtcNow });
                var entity = await _context.Set<T>().SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
                _context.Entry(entity).State = Deleted;
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataDeleteEnd, $"{DataDeleteEnd}"),
                    message: "Deleted entity {@Entity} with predicate {@Predicate} at {@Time}",
                    args: new object[] { entity, predicate.Body, UtcNow });
            }
            catch (Exception e)
            {
                _logger.LogError(
                    eventId: new EventId((int)DataDeleteError, $"{DataDeleteError}"),
                    exception: e,
                    message: "Error deleting with predicate {@Predicate} at {@Time}",
                    args: new object[] { predicate.Body, UtcNow });
                throw;
            }
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
                var queryable = _context.Set<T>();
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataListEnd, $"{DataListEnd}"),
                    message: "Listed type {@Type} at {@Time}",
                    args: new object[] { typeName, UtcNow });
                return queryable;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    eventId: new EventId((int)DataListError, $"{DataListError}"),
                    exception: e,
                    message: "Error listing type {@Type} at {@Time}",
                    args: new object[] { typeName, UtcNow });
                throw;
            }
        }
    }
}
