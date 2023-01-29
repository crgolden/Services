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
    using NHibernate;
    using NHibernate.Linq;
    using static System.GC;
    using static System.String;

    /// <inheritdoc cref="IDataService" />
    [PublicAPI]
    public class NHibernateDataService : IDataService, IDisposable
    {
        private bool _disposedValue;

        /// <summary>Initializes a new instance of the <see cref="NHibernateDataService"/> class.</summary>
        /// <param name="session">The <see cref="ISession"/>.</param>
        /// <param name="nHibernateDataOptions">The <see cref="NHibernateDataOptions"/>.</param>
        /// <param name="name">The name (default is "NHibernate").</param>
        /// <exception cref="ArgumentNullException"><paramref name="session"/> is <see langword="null" /> or <paramref name="nHibernateDataOptions"/> is <see langword="null" /> or <paramref name="name"/> is <see langword="null" />.</exception>
        public NHibernateDataService(ISession session, NHibernateDataOptions nHibernateDataOptions, string name)
        {
            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Options = nHibernateDataOptions ?? throw new ArgumentNullException(nameof(nHibernateDataOptions));
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>Gets the <see cref="NHibernateDataOptions"/>.</summary>
        /// <value>The <see cref="NHibernateDataOptions"/>.</value>
        protected NHibernateDataOptions Options { get; }

        /// <summary>Gets the <see cref="ISession"/>.</summary>
        /// <value>The <see cref="ISession"/>.</value>
        protected ISession Session { get; }

        /// <inheritdoc />
        public virtual async Task<T> CreateAsync<T>(T record, CancellationToken cancellationToken = default)
            where T : class
        {
            if (Options.UseTransaction && Session.GetCurrentTransaction()?.IsActive != true)
            {
                Session.BeginTransaction();
            }

            await Session.SaveAsync(record, cancellationToken).ConfigureAwait(false);
            return record;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> CreateRangeAsync<T>(IEnumerable<T> records, CancellationToken cancellationToken = default)
            where T : class
        {
            if (Options.UseTransaction && Session.GetCurrentTransaction()?.IsActive != true)
            {
                Session.BeginTransaction();
            }

            var enumerated = records.ToArray();
            foreach (var record in enumerated)
            {
                await Session.SaveAsync(record, cancellationToken).ConfigureAwait(false);
            }

            return enumerated;
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where T : class
        {
            if (Options.UseTransaction && Session.GetCurrentTransaction()?.IsActive != true)
            {
                Session.BeginTransaction();
            }

            var record = await Session.Query<T>().SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
            if (record == default)
            {
                return;
            }

            await Session.DeleteAsync(record, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual Task DeleteRangeAsync<T>(IEnumerable<Expression<Func<T, bool>>> predicates, CancellationToken cancellationToken = default)
            where T : class
        {
            if (predicates == default)
            {
                throw new ArgumentNullException(nameof(predicates));
            }

            async Task DeleteRange()
            {
                if (Options.UseTransaction && Session.GetCurrentTransaction()?.IsActive != true)
                {
                    Session.BeginTransaction();
                }

                foreach (var predicate in predicates)
                {
                    var record = await Session.Query<T>().SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
                    if (record == default)
                    {
                        continue;
                    }

                    await Session.DeleteAsync(record, cancellationToken).ConfigureAwait(false);
                }
            }

            return DeleteRange();
        }

        /// <inheritdoc />
        public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var transaction = Session.GetCurrentTransaction();
            if (!Options.UseTransaction || transaction is not { IsActive: true })
            {
                return;
            }

            try
            {
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                transaction.Dispose();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual Task UpdateAsync<T>(Expression<Func<T, bool>> predicate, T record, CancellationToken cancellationToken = default)
            where T : class
        {
            if (Options.UseTransaction && Session.GetCurrentTransaction()?.IsActive != true)
            {
                Session.BeginTransaction();
            }

            return Session.UpdateAsync(record, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task UpdateRangeAsync<T>(IDictionary<Expression<Func<T, bool>>, T> keyValuePairs, CancellationToken cancellationToken = default)
            where T : class
        {
            if (keyValuePairs == default)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }

            async Task UpdateRange()
            {
                if (Options.UseTransaction && Session.GetCurrentTransaction()?.IsActive != true)
                {
                    Session.BeginTransaction();
                }

                foreach (var keyValuePair in keyValuePairs)
                {
                    var record = await Session.Query<T>().SingleOrDefaultAsync(keyValuePair.Key, cancellationToken).ConfigureAwait(false);
                    if (record == default)
                    {
                        continue;
                    }

                    await Session.UpdateAsync(record, cancellationToken).ConfigureAwait(false);
                }
            }

            return UpdateRange();
        }

        /// <inheritdoc />
        public virtual Task<bool> AnyAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return source.AnyAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<bool> AnyAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            return source.AnyAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<decimal?> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, decimal?>> selector, CancellationToken cancellationToken = default)
        {
            return source.AverageAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<decimal> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
        {
            return source.AverageAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<double?> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, double?>> selector, CancellationToken cancellationToken = default)
        {
            return source.AverageAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<double> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, double>> selector, CancellationToken cancellationToken = default)
        {
            return source.AverageAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<float?> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, float?>> selector, CancellationToken cancellationToken = default)
        {
            return source.AverageAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<float> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, float>> selector, CancellationToken cancellationToken = default)
        {
            return source.AverageAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<double?> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, int?>> selector, CancellationToken cancellationToken = default)
        {
            return source.AverageAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<double> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, int>> selector, CancellationToken cancellationToken = default)
        {
            return source.AverageAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<double?> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, long?>> selector, CancellationToken cancellationToken = default)
        {
            return source.AverageAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<double> AverageAsync<T>(IQueryable<T> source, Expression<Func<T, long>> selector, CancellationToken cancellationToken = default)
        {
            return source.AverageAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            return source.CountAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return source.CountAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<T> FirstAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            return source.FirstAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<T> FirstAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return source.FirstAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<T> FirstOrDefaultAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            return source.FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<T> FirstOrDefaultAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return source.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task ForEachAsync<T>(IQueryable<T> source, Action<T> action, CancellationToken cancellationToken = default)
        {
            if (source == default)
            {
                throw new ArgumentNullException(nameof(source));
            }

            async Task ForEach()
            {
                var records = await source.ToListAsync(cancellationToken).ConfigureAwait(false);
                records.ForEach(action);
            }

            return ForEach();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">
        /// <paramref name="keyValues"/> does not contain exactly one key value.
        /// </exception>
        public virtual ValueTask<T> GetAsync<T>(object[] keyValues, CancellationToken cancellationToken = default)
            where T : class
        {
            if (keyValues == default)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }

            if (keyValues.Length != 1)
            {
                throw new ArgumentException("Key values must contain exactly one key value", nameof(keyValues));
            }

            return new ValueTask<T>(Session.GetAsync<T>(keyValues[0], cancellationToken));
        }

        /// <inheritdoc />
        public virtual Task<long> LongCountAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            return source.LongCountAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<long> LongCountAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return source.LongCountAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<T> MaxAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            return source.MaxAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<TResult> MaxAsync<T, TResult>(IQueryable<T> source, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
        {
            return source.MaxAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<T> MinAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            return source.MinAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<TResult> MinAsync<T, TResult>(IQueryable<T> source, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
        {
            return source.MinAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual IQueryable<T> Query<T>()
            where T : class
        {
            return Session.Query<T>();
        }

        /// <inheritdoc />
        public virtual Task<T> SingleAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            return source.SingleAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<T> SingleAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return source.SingleAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<T> SingleOrDefaultAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            return source.SingleOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<T> SingleOrDefaultAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return source.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<decimal?> SumAsync<T>(IQueryable<T> source, Expression<Func<T, decimal?>> selector, CancellationToken cancellationToken = default)
        {
            return source.SumAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<decimal> SumAsync<T>(IQueryable<T> source, Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
        {
            return source.SumAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<double?> SumAsync<T>(IQueryable<T> source, Expression<Func<T, double?>> selector, CancellationToken cancellationToken = default)
        {
            return source.SumAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<double> SumAsync<T>(IQueryable<T> source, Expression<Func<T, double>> selector, CancellationToken cancellationToken = default)
        {
            return source.SumAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<float?> SumAsync<T>(IQueryable<T> source, Expression<Func<T, float?>> selector, CancellationToken cancellationToken = default)
        {
            return source.SumAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<float> SumAsync<T>(IQueryable<T> source, Expression<Func<T, float>> selector, CancellationToken cancellationToken = default)
        {
            return source.SumAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<int?> SumAsync<T>(IQueryable<T> source, Expression<Func<T, int?>> selector, CancellationToken cancellationToken = default)
        {
            return source.SumAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<int> SumAsync<T>(IQueryable<T> source, Expression<Func<T, int>> selector, CancellationToken cancellationToken = default)
        {
            return source.SumAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<long?> SumAsync<T>(IQueryable<T> source, Expression<Func<T, long?>> selector, CancellationToken cancellationToken = default)
        {
            return source.SumAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<long> SumAsync<T>(IQueryable<T> source, Expression<Func<T, long>> selector, CancellationToken cancellationToken = default)
        {
            return source.SumAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<List<T>> ToListAsync<T>(IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            return source.ToListAsync(cancellationToken);
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
