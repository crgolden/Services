namespace Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class HostExtensions
    {
        public static async Task<IHost> MigrateDatabaseAsync(
            this IHost host,
            CancellationToken cancellationToken = default)
        {
            if (host == default)
            {
                throw new ArgumentNullException(nameof(host));
            }

            using (var scope = host.Services.CreateScope())
            {
                await using var context = scope.ServiceProvider.GetRequiredService<DbContext>();
                await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            }

            return host;
        }
    }
}
