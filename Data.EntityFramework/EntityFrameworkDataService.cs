namespace Services
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.EntityFrameworkCore;

    public class EntityFrameworkDataService : IDataService
    {
        private readonly DbContext _context;

        public EntityFrameworkDataService(DbContext context) => _context = context;

        public async Task<T> CreateAsync<T>(
            T entity,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (entity.Equals(default))
            {
                throw new ArgumentNullException(nameof(entity), "Invalid Entity");
            }

            _context.Entry(entity).State = EntityState.Added;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public async Task<T> ReadAsync<T>(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (predicate.Equals(default))
            {
                throw new ArgumentNullException(nameof(predicate), "Invalid Predicate");
            }

            return await _context.Set<T>().SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateAsync<T>(
            Expression<Func<T, bool>> predicate,
            T entity,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (entity.Equals(default))
            {
                throw new ArgumentNullException(nameof(entity), "Invalid Entity");
            }

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAsync<T>(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
            where T : class
        {
            if (predicate.Equals(default))
            {
                throw new ArgumentNullException(nameof(predicate), "Invalid Predicate");
            }

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
