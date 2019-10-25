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
        public Task<T> CreateAsync<T>(
            T? record,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (record == default)
            {
                throw new ArgumentNullException(nameof(record));
            }

            return Create(record, logLevel, cancellationToken);
        }

        /// <inheritdoc />
        public Task<T?> ReadAsync<T>(
            Expression<Func<T, bool>>? expression,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (expression == default)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return Read(expression, logLevel, cancellationToken);
        }

        /// <inheritdoc />
        public Task UpdateAsync<T>(
            Expression<Func<T, bool>>? expression,
            T? record,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (record == default)
            {
                throw new ArgumentNullException(nameof(record));
            }

            return Update(record, logLevel, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteAsync<T>(
            Expression<Func<T, bool>>? expression,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (expression == default)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return Delete(expression, logLevel, cancellationToken);
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

        private async Task<T> Create<T>(
            T record,
            LogLevel logLevel,
            CancellationToken cancellationToken)
            where T : class
        {
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataCreateStart, $"{DataCreateStart}"),
                    message: "Creating entity {@Entity} at {@Time}",
                    args: new object[] { record, UtcNow });
                _context.Entry(record).State = Added;
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataCreateEnd, $"{DataCreateEnd}"),
                    message: "Created entity {@Entity} at {@Time}",
                    args: new object[] { record, UtcNow });
                return record;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    eventId: new EventId((int)DataCreateError, $"{DataCreateError}"),
                    exception: e,
                    message: "Error creating entity {@Entity} at {@Time}",
                    args: new object[] { record, UtcNow });
                throw;
            }
        }

        private async Task<T?> Read<T>(
            Expression<Func<T, bool>> expression,
            LogLevel logLevel,
            CancellationToken cancellationToken)
            where T : class
        {
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataReadStart, $"{DataReadStart}"),
                    message: "Reading predicate {@Predicate} at {@Time}",
                    args: new object[] { expression.Body, UtcNow });
                var entity = await _context.Set<T>().SingleOrDefaultAsync(expression, cancellationToken).ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataReadEnd, $"{DataReadEnd}"),
                    message: "Read entity {@Entity} with predicate {@Predicate} at {@Time}",
                    args: new object[] { entity, expression.Body, UtcNow });
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    eventId: new EventId((int)DataReadError, $"{DataReadError}"),
                    exception: e,
                    message: "Error reading predicate {@Predicate} at {@Time}",
                    args: new object[] { expression.Body, UtcNow });
                throw;
            }
        }

        private async Task Update<T>(
            T record,
            LogLevel logLevel,
            CancellationToken cancellationToken)
            where T : class
        {
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataUpdateStart, $"{DataUpdateStart}"),
                    message: "Updating entity {@Entity} at {@Time}",
                    args: new object[] { record, UtcNow });
                _context.Entry(record).State = Modified;
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataUpdateEnd, $"{DataUpdateEnd}"),
                    message: "Updated entity {@Entity} at {@Time}",
                    args: new object[] { record, UtcNow });
            }
            catch (Exception e)
            {
                _logger.LogError(
                    eventId: new EventId((int)DataUpdateError, $"{DataUpdateError}"),
                    exception: e,
                    message: "Error updating entity {@Entity} at {@Time}",
                    args: new object[] { record, UtcNow });
                throw;
            }
        }

        private async Task Delete<T>(
            Expression<Func<T, bool>> expression,
            LogLevel logLevel,
            CancellationToken cancellationToken)
            where T : class
        {
            try
            {
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataDeleteStart, $"{DataDeleteStart}"),
                    message: "Deleting with predicate {@Predicate} at {@Time}",
                    args: new object[] { expression.Body, UtcNow });
                var entity = await _context.Set<T>().SingleOrDefaultAsync(expression, cancellationToken).ConfigureAwait(false);
                _context.Entry(entity).State = Deleted;
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)DataDeleteEnd, $"{DataDeleteEnd}"),
                    message: "Deleted entity {@Entity} with predicate {@Predicate} at {@Time}",
                    args: new object[] { entity, expression.Body, UtcNow });
            }
            catch (Exception e)
            {
                _logger.LogError(
                    eventId: new EventId((int)DataDeleteError, $"{DataDeleteError}"),
                    exception: e,
                    message: "Error deleting with predicate {@Predicate} at {@Time}",
                    args: new object[] { expression.Body, UtcNow });
                throw;
            }
        }
    }
}
