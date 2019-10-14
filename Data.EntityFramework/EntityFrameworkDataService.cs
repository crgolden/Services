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

    public class EntityFrameworkDataService : IDataService
    {
        private readonly DbContext _context;
        private readonly ILogger<EntityFrameworkDataService> _logger;

        public EntityFrameworkDataService(
            DbContext context,
            ILogger<EntityFrameworkDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<T> CreateAsync<T>(
            T entity,
            CancellationToken cancellationToken = default)
            where T : class
        {
            _context.Entry(entity).State = EntityState.Added;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public async Task<T> ReadAsync<T>(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return await _context.Set<T>().SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateAsync<T>(
            Expression<Func<T, bool>> predicate,
            T entity,
            CancellationToken cancellationToken = default)
            where T : class
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAsync<T>(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var entity = _context.Set<T>().SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public IQueryable<T> List<T>()
            where T : class
        {
            return _context.Set<T>();
        }
    }
}
