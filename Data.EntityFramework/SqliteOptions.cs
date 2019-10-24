namespace Services
{
    using System.Text.Json.Serialization;
    using Microsoft.Data.Sqlite;
    using static Microsoft.Data.Sqlite.SqliteCacheMode;
    using static Microsoft.Data.Sqlite.SqliteOpenMode;

    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.data.sqlite.sqliteconnectionstringbuilder
    public class SqliteOptions
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SqliteCacheMode Cache { get; set; } = Default;

        public string? DataSource { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SqliteOpenMode Mode { get; set; } = ReadWriteCreate;

        public string GetConnectionString()
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = DataSource,
                Cache = Cache,
                Mode = Mode
            };
            return builder.ConnectionString;
        }
    }
}
