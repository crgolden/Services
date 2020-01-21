namespace Services
{
    using System.Text.Json.Serialization;
    using JetBrains.Annotations;
    using Microsoft.Data.SqlClient;
    using static Microsoft.Data.SqlClient.ApplicationIntent;

    /// <summary>Configuration settings for the <see cref="EntityFrameworkDataService"/> class.</summary>
    [PublicAPI]
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

        /// <summary>Gets or sets the application intent.</summary>
        /// <value>The application intent.</value>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApplicationIntent ApplicationIntent { get; set; } = ReadWrite;

        public string UserId { get; set; }

        /// <summary>
        /// <para>Gets the connection string.</para>
        /// <para>See https://docs.microsoft.com/en-us/dotnet/api/microsoft.data.sqlclient.sqlconnectionstringbuilder.</para>
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get
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
                    ApplicationIntent = ApplicationIntent
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
    }
}
