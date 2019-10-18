namespace Services
{
    using System;
    using Microsoft.Data.Sqlite;

    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.data.sqlite.sqliteconnectionstringbuilder
    public class SqliteOptions
    {
        public string? Cache { get; set; } = "Default";

        public string? DataSource { get; set; }

        public string? Mode { get; set; } = "ReadWriteCreate";

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
