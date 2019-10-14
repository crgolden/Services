namespace Services
{
    using System;
    using Microsoft.Data.SqlClient;
    using Microsoft.Data.Sqlite;

    public class EntityFrameworkDataOptions
    {
        public string DatabaseType { get; set; }

        public bool SeedData { get; set; }

        public string AssemblyName { get; set; }

        public bool UseLazyLoadingProxies { get; set; }

        public SqlServerOptions SqlServerOptions { get; set; }

        public SqliteOptions SqliteOptions { get; set; }
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnectionstringbuilder
    /// </summary>
    public class SqlServerOptions
    {
        public int ConnectTimeout { get; set; }

        public string DataSource { get; set; }

        public bool Encrypt { get; set; }

        public string InitialCatalog { get; set; }

        public bool IntegratedSecurity { get; set; }

        public bool MultipleActiveResultSets { get; set; }

        public string Password { get; set; }

        public bool PersistSecurityInfo { get; set; }

        public bool TrustServerCertificate { get; set; }

        public string UserId { get; set; }

        public string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                ConnectTimeout = ConnectTimeout,
                DataSource = DataSource,
                Encrypt = Encrypt,
                InitialCatalog = InitialCatalog,
                IntegratedSecurity = IntegratedSecurity,
                MultipleActiveResultSets = MultipleActiveResultSets,
                PersistSecurityInfo = PersistSecurityInfo,
                TrustServerCertificate = TrustServerCertificate,
            };
            if (builder.IntegratedSecurity)
            {
                return builder.ConnectionString;
            }

            builder.Password = Password;
            builder.UserID = UserId;

            return builder.ConnectionString;
        }
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/api/microsoft.data.sqlite.sqliteconnectionstringbuilder
    /// </summary>
    public class SqliteOptions
    {
        public string Cache { get; set; } = "Default";

        public string DataSource { get; set; }

        public string Mode { get; set; } = "ReadWriteCreate";

        public string GetConnectionString()
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = DataSource,
                Cache = (SqliteCacheMode)Enum.Parse(typeof(SqliteCacheMode), Cache, true),
                Mode = (SqliteOpenMode)Enum.Parse(typeof(SqliteOpenMode), Mode, true)
            };
            return builder.ConnectionString;
        }
    }
}