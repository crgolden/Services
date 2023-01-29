namespace System
{
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Threading;
    using Threading.Tasks;

    /// <summary>A class with methods that extend <see cref="IServiceProvider"/>.</summary>
    [PublicAPI]
    public static class ServiceProviderExtensions
    {
        /// <summary>Migrates the database.</summary>
        /// <param name="provider">The provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <paramref name="provider"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> is <see langword="null"/>.</exception>
        public static Task<IServiceProvider> MigrateDatabaseAsync(this IServiceProvider provider, CancellationToken cancellationToken = default)
        {
            if (provider == default)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            async Task<IServiceProvider> MigrateDatabase()
            {
                using var scope = provider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DbContext>();
                await using (context.ConfigureAwait(false))
                {
                    await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
                }

                return provider;
            }

            return MigrateDatabase();
        }
    }
}
