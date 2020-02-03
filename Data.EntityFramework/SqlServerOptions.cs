namespace Services
{
    using System.Text.Json.Serialization;
    using JetBrains.Annotations;
    using Microsoft.Data.SqlClient;
    using static System.String;
    using static Microsoft.Data.SqlClient.ApplicationIntent;

    /// <summary>Configuration settings for the <see cref="EntityFrameworkDataService"/> class.</summary>
    [PublicAPI]
    public class SqlServerOptions
    {
        /// <summary>Gets or sets the application intent.</summary>
        /// <value>The application intent.</value>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApplicationIntent? ApplicationIntent { get; set; } = ReadWrite;

        /// <summary>Gets or sets the name of the application.</summary>
        /// <value>The name of the application.</value>
        public string ApplicationName { get; set; }

        /// <summary>Gets or sets the attach database filename.</summary>
        /// <value>The attach database filename.</value>
        public string AttachDBFilename { get; set; }

        /// <summary>Gets or sets the authentication.</summary>
        /// <value>The authentication.</value>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SqlAuthenticationMethod? Authentication { get; set; }

        /// <summary>Gets or sets the connect retry count.</summary>
        /// <value>The connect retry count.</value>
        public int? ConnectRetryCount { get; set; }

        /// <summary>Gets or sets the connect retry interval.</summary>
        /// <value>The connect retry interval.</value>
        public int? ConnectRetryInterval { get; set; }

        /// <summary>Gets or sets the connect timeout.</summary>
        /// <value>The connect timeout.</value>
        public int? ConnectTimeout { get; set; }

        /// <summary>Gets or sets the current language.</summary>
        /// <value>The current language.</value>
        public string CurrentLanguage { get; set; }

        /// <summary>Gets or sets the data source.</summary>
        /// <value>The data source.</value>
        public string DataSource { get; set; }

        /// <summary>Gets or sets a value indicating whether to encrypt.</summary>
        /// <value>
        /// <c>true</c> if encrypt; otherwise, <c>false</c>.</value>
        public bool? Encrypt { get; set; }

        /// <summary>Gets or sets the enlist.</summary>
        /// <value>The enlist.</value>
        public bool? Enlist { get; set; }

        /// <summary>Gets or sets the failover partner.</summary>
        /// <value>The failover partner.</value>
        public string FailoverPartner { get; set; }

        /// <summary>Gets or sets the initial catalog.</summary>
        /// <value>The initial catalog.</value>
        public string InitialCatalog { get; set; }

        /// <summary>Gets or sets the integrated security.</summary>
        /// <value>The integrated security.</value>
        public bool? IntegratedSecurity { get; set; }

        /// <summary>Gets or sets the load balance timeout.</summary>
        /// <value>The load balance timeout.</value>
        public int? LoadBalanceTimeout { get; set; }

        /// <summary>Gets or sets the maximum size of the pool.</summary>
        /// <value>The maximum size of the pool.</value>
        public int? MaxPoolSize { get; set; }

        /// <summary>Gets or sets the minimum size of the pool.</summary>
        /// <value>The minimum size of the pool.</value>
        public int? MinPoolSize { get; set; }

        /// <summary>Gets or sets the multiple active result sets.</summary>
        /// <value>The multiple active result sets.</value>
        public bool? MultipleActiveResultSets { get; set; }

        /// <summary>Gets or sets the multi subnet failover.</summary>
        /// <value>The multi subnet failover.</value>
        public bool? MultiSubnetFailover { get; set; }

        /// <summary>Gets or sets the size of the packet.</summary>
        /// <value>The size of the packet.</value>
        public int? PacketSize { get; set; }

        /// <summary>Gets or sets the password.</summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>Gets or sets the persist security information.</summary>
        /// <value>The persist security information.</value>
        public bool? PersistSecurityInfo { get; set; }

        /// <summary>Gets or sets the pooling.</summary>
        /// <value>The pooling.</value>
        public bool? Pooling { get; set; }

        /// <summary>Gets or sets the replication.</summary>
        /// <value>The replication.</value>
        public bool? Replication { get; set; }

        /// <summary>Gets or sets the transaction binding.</summary>
        /// <value>The transaction binding.</value>
        public string TransactionBinding { get; set; }

        /// <summary>Gets or sets the trust server certificate.</summary>
        /// <value>The trust server certificate.</value>
        public bool? TrustServerCertificate { get; set; }

        /// <summary>Gets or sets the type system version.</summary>
        /// <value>The type system version.</value>
        public string TypeSystemVersion { get; set; }

        /// <summary>Gets or sets the user identifier.</summary>
        /// <value>The user identifier.</value>
        public string UserId { get; set; }

        /// <summary>Gets or sets the user instance.</summary>
        /// <value>The user instance.</value>
        public bool? UserInstance { get; set; }

        /// <summary>Gets or sets the workstation identifier.</summary>
        /// <value>The workstation identifier.</value>
        public string WorkstationId { get; set; }

        /// <summary>
        /// <para>Gets the connection string.</para>
        /// <para>See https://docs.microsoft.com/en-us/dotnet/api/microsoft.data.sqlclient.sqlconnectionstringbuilder.</para>
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get
            {
                var builder = new SqlConnectionStringBuilder();
                if (ApplicationIntent.HasValue)
                {
                    builder.ApplicationIntent = ApplicationIntent.Value;
                }

                if (!IsNullOrWhiteSpace(ApplicationName))
                {
                    builder.ApplicationName = ApplicationName;
                }

                if (!IsNullOrWhiteSpace(AttachDBFilename))
                {
                    builder.AttachDBFilename = AttachDBFilename;
                }

                if (Authentication.HasValue)
                {
                    builder.Authentication = Authentication.Value;
                }

                if (ConnectRetryCount.HasValue)
                {
                    builder.ConnectRetryCount = ConnectRetryCount.Value;
                }

                if (ConnectRetryInterval.HasValue)
                {
                    builder.ConnectRetryInterval = ConnectRetryInterval.Value;
                }

                if (ConnectTimeout.HasValue)
                {
                    builder.ConnectTimeout = ConnectTimeout.Value;
                }

                if (!IsNullOrWhiteSpace(CurrentLanguage))
                {
                    builder.CurrentLanguage = CurrentLanguage;
                }

                if (!IsNullOrWhiteSpace(DataSource))
                {
                    builder.DataSource = DataSource;
                }

                if (Encrypt.HasValue)
                {
                    builder.Encrypt = Encrypt.Value;
                }

                if (!IsNullOrWhiteSpace(FailoverPartner))
                {
                    builder.FailoverPartner = FailoverPartner;
                }

                if (!IsNullOrWhiteSpace(InitialCatalog))
                {
                    builder.InitialCatalog = InitialCatalog;
                }

                if (IntegratedSecurity.HasValue)
                {
                    builder.IntegratedSecurity = IntegratedSecurity.Value;
                }

                if (LoadBalanceTimeout.HasValue)
                {
                    builder.LoadBalanceTimeout = LoadBalanceTimeout.Value;
                }

                if (MaxPoolSize.HasValue)
                {
                    builder.MaxPoolSize = MaxPoolSize.Value;
                }

                if (MinPoolSize.HasValue)
                {
                    builder.MinPoolSize = MinPoolSize.Value;
                }

                if (MultipleActiveResultSets.HasValue)
                {
                    builder.MultipleActiveResultSets = MultipleActiveResultSets.Value;
                }

                if (MultiSubnetFailover.HasValue)
                {
                    builder.MultiSubnetFailover = MultiSubnetFailover.Value;
                }

                if (PacketSize.HasValue)
                {
                    builder.PacketSize = PacketSize.Value;
                }

                if (!IsNullOrWhiteSpace(Password))
                {
                    builder.Password = Password;
                }

                if (PersistSecurityInfo.HasValue)
                {
                    builder.PersistSecurityInfo = PersistSecurityInfo.Value;
                }

                if (Pooling.HasValue)
                {
                    builder.Pooling = Pooling.Value;
                }

                if (Replication.HasValue)
                {
                    builder.Replication = Replication.Value;
                }

                if (!IsNullOrWhiteSpace(TransactionBinding))
                {
                    builder.TransactionBinding = TransactionBinding;
                }

                if (TrustServerCertificate.HasValue)
                {
                    builder.TrustServerCertificate = TrustServerCertificate.Value;
                }

                if (!IsNullOrWhiteSpace(UserId))
                {
                    builder.UserID = UserId;
                }

                if (UserInstance.HasValue)
                {
                    builder.UserInstance = UserInstance.Value;
                }

                if (!IsNullOrWhiteSpace(WorkstationId))
                {
                    builder.WorkstationID = WorkstationId;
                }

                return builder.ConnectionString;
            }
        }
    }
}
