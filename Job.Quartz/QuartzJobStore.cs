namespace Services
{
    using System;
    using System.Collections.Specialized;
    using JetBrains.Annotations;
    using Quartz;

    /// <summary>Helper class for configuring the <see cref="QuartzJobService"/>.</summary>
    // https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/job-stores.html
    [PublicAPI]
    public static class QuartzJobStore
    {
        /// <summary>The standard ADO delegate.</summary>
        public const string StandardAdoDelegate = "Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz";

        /// <summary>The standard lock handler.</summary>
        public const string StandardLockHandler = "Quartz.Impl.AdoJobStore.StdRowLockSemaphore, Quartz";

        /// <summary>The SQL Server provider.</summary>
        public const string SqlServerProvider = "SqlServer";

        /// <summary>The SQL Server delegate.</summary>
        public const string SqlServerDelegate = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz";

        /// <summary>The SQL server lock handler.</summary>
        public const string SqlServerLockHandler = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz";

        /// <summary>The Oracle provider.</summary>
        public const string OracleProvider = "OracleODP";

        /// <summary>The Oracle delegate.</summary>
        public const string OracleDelegate = "Quartz.Impl.AdoJobStore.OracleDelegate, Quartz";

        /// <summary>MySQL provider.</summary>
        public const string MySqlProvider = "MySql";

        /// <summary>MySQL delegate.</summary>
        public const string MySqlDelegate = "Quartz.Impl.AdoJobStore.MySQLDelegate, Quartz";

        /// <summary>The SQLite provider.</summary>
        public const string SqliteProvider = "SQLite";

        /// <summary>The SQLite delegate.</summary>
        public const string SqliteDelegate = "Quartz.Impl.AdoJobStore.SQLiteDelegate, Quartz";

        /// <summary>The Firebird provider.</summary>
        public const string FirebirdProvider = "Firebird";

        /// <summary>The Firebird delegate.</summary>
        public const string FirebirdDelegate = "Quartz.Impl.AdoJobStore.FirebirdDelegate, Quartz";

        /// <summary>The NPGSQL provider.</summary>
        public const string NpgsqlProvider = "Npgsql";

        /// <summary>The NPGSQL delegate.</summary>
        public const string NpgsqlDelegate = "Quartz.Impl.AdoJobStore.PostgreSQLDelegate, Quartz";

        /// <summary>Get the job store properties for initializing an <see cref="ISchedulerFactory"/>.</summary>
        /// <param name="options">The options.</param>
        /// <returns>The job store properties.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="options"/>is <see langword="null"/>.</exception>
        public static NameValueCollection JobStoreProps(QuartzJobStoreOptions options) => options == default
            ? throw new ArgumentNullException(nameof(options))
            : new NameValueCollection
            {
                { "quartz.scheduler.instanceName", options.InstanceName },
                { "quartz.scheduler.instanceId", options.InstanceId },
                { "quartz.jobStore.type", options.Type },
                { "quartz.jobStore.driverDelegateType", options.DriverDelegateType },
                { "quartz.jobStore.dataSource", options.DataSource },
                { "quartz.jobStore.tablePrefix", options.TablePrefix },
                { "quartz.jobStore.useProperties", options.UseProperties },
                { "quartz.jobStore.lockHandler.type", options.LockHandlerType },
                { $"quartz.dataSource.{options.DataSource}.connectionString", options.DataSourceConnectionString },
                { $"quartz.dataSource.{options.DataSource}.provider", options.DataSourceProvider }
            };
    }
}
