namespace Services
{
    using System;
    using Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using static System.Enum;
    using static System.String;
    using static System.TimeSpan;
    using static Common.DatabaseType;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkDataService<T>(
            this IServiceCollection services,
            IConfiguration? configuration)
            where T : DbContext
        {
            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection(nameof(EntityFrameworkDataOptions));
            if (!section.Exists())
            {
                throw new ArgumentException(
                    message: $"{nameof(EntityFrameworkDataOptions)} section doesn't exist",
                    paramName: nameof(configuration));
            }

            services.Configure<EntityFrameworkDataOptions>(section);
            var options = section.Get<EntityFrameworkDataOptions>();
            if (options == default ||
                !TryParse<DatabaseType>(options.DatabaseType, true, out var databaseType))
            {
                throw new ArgumentException(
                    message: $"{nameof(EntityFrameworkDataOptions)} section is invalid",
                    paramName: nameof(configuration));
            }

            var builderAction = GetBuilderAction(databaseType, options);
            services.AddDbContext<T>(builderAction);
            services.AddScoped<DbContext, T>();
            return services;
        }

        private static Action<DbContextOptionsBuilder> GetBuilderAction(
            DatabaseType? databaseType,
            EntityFrameworkDataOptions? options)
        {
            return databaseType switch
            {
                SqlServer => GetSqlServerBuilder(options),
                Sqlite => GetSqliteBuilder(options),
                _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
            };
        }

        private static Action<DbContextOptionsBuilder> GetSqlServerBuilder(
            EntityFrameworkDataOptions? options)
        {
            if (options?.SqlServerOptions == default)
            {
                throw new ArgumentException(
                    message: $"{nameof(SqlServerOptions)} section is invalid",
                    paramName: nameof(options));
            }

            return builder =>
            {
                builder.UseSqlServer(
                    connectionString: options.SqlServerOptions.GetConnectionString(),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 15,
                            maxRetryDelay: FromSeconds(30),
                            errorNumbersToAdd: default);
                        if (IsNullOrEmpty(options.AssemblyName))
                        {
                            return;
                        }

                        sqlOptions.MigrationsAssembly(options.AssemblyName);
                    });
                if (options.UseLazyLoadingProxies)
                {
                    builder.UseLazyLoadingProxies();
                }
            };
        }

        private static Action<DbContextOptionsBuilder> GetSqliteBuilder(
            EntityFrameworkDataOptions? options)
        {
            if (options?.SqliteOptions == default)
            {
                throw new ArgumentException(
                    message: $"{nameof(SqliteOptions)} section is invalid",
                    paramName: nameof(options));
            }

            return builder =>
            {
                builder.UseSqlite(
                    connectionString: options.SqliteOptions.GetConnectionString(),
                    sqliteOptionsAction: sqliteOptions =>
                    {
                        if (IsNullOrEmpty(options.AssemblyName))
                        {
                            return;
                        }

                        sqliteOptions.MigrationsAssembly(options.AssemblyName);
                    });
                if (options.UseLazyLoadingProxies)
                {
                    builder.UseLazyLoadingProxies();
                }
            };
        }
    }
}
