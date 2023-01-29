namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Services;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using static System.Linq.Enumerable;
    using static System.String;
    using static System.Threading.Tasks.Task;

    /// <inheritdoc />
    [PublicAPI]
    public class EntityFrameworkDataService : IDataService
    {
        /// <summary>Initializes a new instance of the <see cref="EntityFrameworkDataService"/> class.</summary>
        /// <param name="context">The <see cref="DbContext"/>.</param>
        /// <param name="name">The name (default is "EntityFrameworkCore").</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null" />
        /// or
        /// <paramref name="name"/> is <see langword="null" />.</exception>
        public EntityFrameworkDataService(DbContext context, string name = nameof(EntityFrameworkDataService))
        {
            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Context = context ?? throw new ArgumentNullException(nameof(context));
            Name = name;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>Gets the <see cref="DbContext"/>.</summary>
        /// <value>The <see cref="DbContext"/>.</value>
        protected DbContext Context { get; }

        /// <inheritdoc />
        public virtual Task<T> CreateAsync<T>(T record, CancellationToken cancellationToken = default)
            where T : class
        {
            if (record == default)
            {
                throw new ArgumentNullException(nameof(record));
            }

            Context.Set<T>().Add(record);
            return FromResult(record);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<T>> CreateRangeAsync<T>(IEnumerable<T> records, CancellationToken cancellationToken = default)
            where T : class
        {
            if (records == default)
            {
                throw new ArgumentNullException(nameof(records));
            }

            var enumerated = records.ToArray();
            Context.Set<T>().AddRange(enumerated);
            return FromResult(enumerated.AsEnumerable());
        }

        /// <inheritdoc />
        public virtual Task DeleteAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where T : class
        {
            if (predicate == default)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            async Task Delete()
            {
                var record = await Context.Set<T>().SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
                if (record == default)
                {
                    return;
                }

                Context.Set<T>().Remove(record);
            }

            return Delete();
        }

        /// <inheritdoc />
        public virtual Task DeleteRangeAsync<T>(IEnumerable<Expression<Func<T, bool>>> predicates, CancellationToken cancellationToken = default)
            where T : class
        {
            if (predicates == default)
            {
                throw new ArgumentNullException(nameof(predicates));
            }

            var query = predicates.Aggregate(Empty<T>().AsQueryable(), (current, next) => current.Union(current.Where(next)));
            Context.Set<T>().RemoveRange(query);
            return CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task UpdateAsync<T>(Expression<Func<T, bool>> predicate, T record, CancellationToken cancellationToken = default)
            where T : class
        {
            if (record == default)
            {
                throw new ArgumentNullException(nameof(record));
            }

            Context.Set<T>().Update(record);
            return CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task UpdateRangeAsync<T>(IDictionary<Expression<Func<T, bool>>, T> keyValuePairs, CancellationToken cancellationToken = default)
            where T : class
        {
            if (keyValuePairs == default)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }

            Context.Set<T>().UpdateRange(keyValuePairs.Values);
            return CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task<bool> AnyAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.AnyAsync(cancellationToken);
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

            return source.AnyAsync(predicate, cancellationToken);
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

            return source.AverageAsync(selector, cancellationToken);
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

            return source.AverageAsync(selector, cancellationToken);
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

            return source.AverageAsync(selector, cancellationToken);
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

            return source.AverageAsync(selector, cancellationToken);
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

            return source.AverageAsync(selector, cancellationToken);
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

            return source.AverageAsync(selector, cancellationToken);
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

            return source.AverageAsync(selector, cancellationToken);
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

            return source.AverageAsync(selector, cancellationToken);
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

            return source.AverageAsync(selector, cancellationToken);
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

            return source.AverageAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.CountAsync(cancellationToken);
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

            return source.CountAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<T> FirstAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.FirstAsync(cancellationToken);
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

            return source.FirstAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<T> FirstOrDefaultAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.FirstOrDefaultAsync(cancellationToken);
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

            return source.FirstOrDefaultAsync(predicate, cancellationToken);
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

            return source.ForEachAsync(action, cancellationToken);
        }

        /// <inheritdoc />
        public virtual ValueTask<T> GetAsync<T>(object[] keyValues, CancellationToken cancellationToken = default)
            where T : class
        {
            if (keyValues == default)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            if (keyValues.Length == 0)
            {
                throw new ArgumentException("Key values cannot be empty", nameof(keyValues));
            }

            return Context.Set<T>().FindAsync(keyValues, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<long> LongCountAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.LongCountAsync(cancellationToken);
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

            return source.LongCountAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<T> MaxAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.MaxAsync(cancellationToken);
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

            return source.MaxAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<T> MinAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.MinAsync(cancellationToken);
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

            return source.MinAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual IQueryable<T> Query<T>()
            where T : class
        {
            return Context.Set<T>();
        }

        /// <inheritdoc />
        public virtual Task<T> SingleAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.SingleAsync(cancellationToken);
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

            return source.SingleAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<T> SingleOrDefaultAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.SingleOrDefaultAsync(cancellationToken);
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

            return source.SingleOrDefaultAsync(predicate, cancellationToken);
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

            return source.SumAsync(selector, cancellationToken);
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

            return source.SumAsync(selector, cancellationToken);
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

            return source.SumAsync(selector, cancellationToken);
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

            return source.SumAsync(selector, cancellationToken);
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

            return source.SumAsync(selector, cancellationToken);
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

            return source.SumAsync(selector, cancellationToken);
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

            return source.SumAsync(selector, cancellationToken);
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

            return source.SumAsync(selector, cancellationToken);
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

            return source.SumAsync(selector, cancellationToken);
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

            return source.SumAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<List<T>> ToListAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.ToListAsync(cancellationToken);
        }
    }
}
