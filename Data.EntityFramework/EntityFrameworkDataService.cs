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
    using static Common.DataServiceType;
    using static Microsoft.EntityFrameworkCore.EntityState;

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

        public string? Name { get; set; }

        public DataServiceType Type => EntityFramework;

        public async Task<T> CreateAsync<T>(
            T? entity,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _context.Entry(entity).State = Added;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public async Task<T?> ReadAsync<T>(
            Expression<Func<T, bool>>? predicate,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return await _context.Set<T>().SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateAsync<T>(
            Expression<Func<T, bool>>? predicate,
            T? entity,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _context.Entry(entity).State = Modified;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAsync<T>(
            Expression<Func<T, bool>>? predicate,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var entity = _context.Set<T>().SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
            _context.Entry(entity).State = Deleted;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public IQueryable<T> List<T>()
            where T : class
        {
            return _context.Set<T>();
        }
    }
}
