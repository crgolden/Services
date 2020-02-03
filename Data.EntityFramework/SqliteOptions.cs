namespace Services
{
    using System.Text.Json.Serialization;
    using JetBrains.Annotations;
    using Microsoft.Data.Sqlite;
    using static System.String;
    using static Microsoft.Data.Sqlite.SqliteCacheMode;
    using static Microsoft.Data.Sqlite.SqliteOpenMode;

    /// <summary>Configuration settings for the <see cref="EntityFrameworkDataService"/> class.</summary>
    [PublicAPI]
    public class SqliteOptions
    {
        /// <summary>Gets or sets the database file.</summary>
        /// <value>The database file.</value>
        public string DataSource { get; set; }

        /// <summary>Gets or sets the connection mode.</summary>
        /// <value>The connection mode.</value>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SqliteOpenMode? Mode { get; set; } = ReadWriteCreate;

        /// <summary>Gets or sets the caching mode used by the connection.</summary>
        /// <value>The caching mode used by the connection.</value>
        /// <seealso href="http://sqlite.org/sharedcache.html">SQLite Shared-Cache Mode</seealso>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SqliteCacheMode? Cache { get; set; } = Default;

        /// <summary>
        /// Gets or sets the encryption key. Warning, this has no effect when the native SQLite library doesn't
        /// support encryption. When specified, <c>PRAGMA key</c> is sent immediately after opening the connection.
        /// </summary>
        /// <value>The encryption key.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable foreign key constraints. When true,
        /// <c>PRAGMA foreign_keys = 1</c> is sent immediately after opening the connection. When false,
        /// <c>PRAGMA foreign_keys = 0</c> is sent. When null, no pragma is sent. There is no need enable foreign
        /// keys if, like in e_sqlite3, SQLITE_DEFAULT_FOREIGN_KEYS was used to compile the native library.
        /// </summary>
        /// <value>A value indicating whether to enable foreign key constraints.</value>
        public bool? ForeignKeys { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable recursive triggers. When true,
        /// <c>PRAGMA recursive_triggers</c> is sent immediately after opening the connection. When false, no pragma
        /// is sent.
        /// </summary>
        /// <value>A value indicating whether to enable recursive triggers.</value>
        public bool? RecursiveTriggers { get; set; }

        /// <summary>
        /// <para>Gets the connection string.</para>
        /// <para>See https://docs.microsoft.com/en-us/dotnet/api/microsoft.data.sqlite.sqliteconnectionstringbuilder.</para>
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get
            {
                var builder = new SqliteConnectionStringBuilder();
                if (!IsNullOrWhiteSpace(DataSource))
                {
                    builder.DataSource = DataSource;
                }

                if (Mode.HasValue)
                {
                    builder.Mode = Mode.Value;
                }

                if (Cache.HasValue)
                {
                    builder.Cache = Cache.Value;
                }

                if (!IsNullOrWhiteSpace(Password))
                {
                    builder.Password = Password;
                }

                if (ForeignKeys.HasValue)
                {
                    builder.ForeignKeys = ForeignKeys.Value;
                }

                if (RecursiveTriggers.HasValue)
                {
                    builder.RecursiveTriggers = RecursiveTriggers.Value;
                }

                return builder.ConnectionString;
            }
        }
    }
}
