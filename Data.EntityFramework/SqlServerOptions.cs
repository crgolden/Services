namespace Services
{
    using Microsoft.Data.SqlClient;

    // https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnectionstringbuilder
    public class SqlServerOptions
    {
        public int ConnectTimeout { get; set; }

        public string? DataSource { get; set; }

        public bool Encrypt { get; set; }

        public string? InitialCatalog { get; set; }

        public bool IntegratedSecurity { get; set; }

        public bool MultipleActiveResultSets { get; set; }

        public string? Password { get; set; }

        public bool PersistSecurityInfo { get; set; }

        public bool TrustServerCertificate { get; set; }

        public string? UserId { get; set; }

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
}