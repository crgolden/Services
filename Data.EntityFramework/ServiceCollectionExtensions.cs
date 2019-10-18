namespace Services
{
    using System;
    using Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

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
                throw new Exception($"{nameof(EntityFrameworkDataOptions)} section doesn't exist");
            }

            services.Configure<EntityFrameworkDataOptions>(section);
            var entityFrameworkDataOptions = section.Get<EntityFrameworkDataOptions>();
            if (entityFrameworkDataOptions == default ||
                !Enum.TryParse<DatabaseType>(entityFrameworkDataOptions.DatabaseType, true, out var databaseType))
            {
                throw new Exception($"{nameof(EntityFrameworkDataOptions)} section is invalid");
            }

            var builderAction = GetBuilderAction(databaseType, entityFrameworkDataOptions);
            services.AddDbContext<T>(builderAction);
            services.AddScoped<DbContext, T>();
            return services;
        }

        private static Action<DbContextOptionsBuilder> GetBuilderAction(
            DatabaseType databaseType,
            EntityFrameworkDataOptions entityFrameworkDataOptions)
        {
            Action<DbContextOptionsBuilder> builderAction;
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    if (entityFrameworkDataOptions.SqlServerOptions == default)
                    {
                        throw new Exception($"{nameof(SqlServerOptions)} section is invalid");
                    }

                    builderAction = builder =>
                    {
                        builder.UseSqlServer(
                            connectionString: entityFrameworkDataOptions.SqlServerOptions.GetConnectionString(),
                            sqlServerOptionsAction: sqlOptions =>
                            {
                                sqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: 15,
                                    maxRetryDelay: TimeSpan.FromSeconds(30),
                                    errorNumbersToAdd: null);
                                if (string.IsNullOrEmpty(entityFrameworkDataOptions.AssemblyName))
                                {
                                    return;
                                }

                                sqlOptions.MigrationsAssembly(entityFrameworkDataOptions.AssemblyName);
                            });
                        if (entityFrameworkDataOptions.UseLazyLoadingProxies)
                        {
                            builder.UseLazyLoadingProxies();
                        }
                    };
                    break;
                case DatabaseType.Sqlite:
                    if (entityFrameworkDataOptions.SqliteOptions == default)
                    {
                        throw new Exception($"{nameof(SqliteOptions)} section is invalid");
                    }

                    builderAction = builder =>
                    {
                        builder.UseSqlite(
                            connectionString: entityFrameworkDataOptions.SqliteOptions.GetConnectionString(),
                            sqliteOptionsAction: sqliteOptions =>
                            {
                                if (string.IsNullOrEmpty(entityFrameworkDataOptions.AssemblyName))
                                {
                                    return;
                                }

                                sqliteOptions.MigrationsAssembly(entityFrameworkDataOptions.AssemblyName);
                            });
                        if (entityFrameworkDataOptions.UseLazyLoadingProxies)
                        {
                            builder.UseLazyLoadingProxies();
                        }
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType));
            }

            return builderAction;
        }
    }
}
