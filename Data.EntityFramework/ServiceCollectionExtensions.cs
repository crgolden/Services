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
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
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
                case DatabaseType.Sqlite:
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType));
            }
        }
    }
}
