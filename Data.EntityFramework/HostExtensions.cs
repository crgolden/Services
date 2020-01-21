namespace Microsoft.Extensions.Hosting
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DependencyInjection;
    using EntityFrameworkCore;

    public static class HostExtensions
    {
        public static Task<IHost> MigrateDatabaseAsync(this IHost host, CancellationToken cancellationToken = default)
        {
            if (host == default)
            {
                throw new ArgumentNullException(nameof(host));
            }

            async Task<IHost> MigrateDatabase()
            {
                using (var scope = host.Services.CreateScope())
                {
                    await using var context = scope.ServiceProvider.GetRequiredService<DbContext>();
                    await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
                }

                return host;
            }

            return MigrateDatabase();
        }
    }
}
