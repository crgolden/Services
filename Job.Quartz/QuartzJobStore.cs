namespace Services
{
    using System;
    using System.Collections.Specialized;

    // https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/job-stores.html
    public static class QuartzJobStore
    {
        public const string StandardAdoDelegate = "Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz";

        public const string StandardLockHandler = "Quartz.Impl.AdoJobStore.StdRowLockSemaphore, Quartz";

        public const string SqlServerProvider = "SqlServer";

        public const string SqlServerDelegate = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz";

        public const string SqlServerLockHandler = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz";

        public const string OracleProvider = "OracleODP";

        public const string OracleDelegate = "Quartz.Impl.AdoJobStore.OracleDelegate, Quartz";

        public const string MySqlProvider = "MySql";

        public const string MySqlDelegate = "Quartz.Impl.AdoJobStore.MySQLDelegate, Quartz";

        public const string SqliteProvider = "SQLite";

        public const string SqliteDelegate = "Quartz.Impl.AdoJobStore.SQLiteDelegate, Quartz";

        public const string FirebirdProvider = "Firebird";

        public const string FirebirdDelegate = "Quartz.Impl.AdoJobStore.FirebirdDelegate, Quartz";

        public const string NpgsqlProvider = "Npgsql";

        public const string NpgsqlDelegate = "Quartz.Impl.AdoJobStore.PostgreSQLDelegate, Quartz";

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
