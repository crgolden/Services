namespace Services.Options
{
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;
    using static System.String;
    using static MongoDB.Driver.MongoClientSettings;
    using static MongoDB.Driver.MongoCredential;
    using static MongoDB.Driver.MongoUrl;

    [UsedImplicitly]
    internal class ConfigureOptions : IConfigureNamedOptions<MongoDataOptions>
    {
        public void Configure(MongoDataOptions options)
        {
            Configure(nameof(MongoDB), options);
        }

        public void Configure(string name, MongoDataOptions options)
        {
            if (options.MongoClientSettings != default)
            {
                if (IsNullOrWhiteSpace(options.MongoClientSettings.ApplicationName))
                {
                    options.MongoClientSettings.ApplicationName = name;
                }

                if (options.MongoClientSettings.Credential == default &&
                    !IsNullOrWhiteSpace(options.Username) &&
                    !IsNullOrWhiteSpace(options.Password) &&
                    !IsNullOrWhiteSpace(options.AuthDatabaseName))
                {
                    var credential = CreateCredential(options.AuthDatabaseName, options.Username, options.Password);
                    options.MongoClientSettings.Credential = credential;
                }

                if (!IsNullOrWhiteSpace(options.Host))
                {
                    options.MongoClientSettings.Server = options.Port.HasValue
                        ? new MongoServerAddress(options.Host, options.Port.Value)
                        : new MongoServerAddress(options.Host);
                }

                if (options.MongoUrl == default)
                {
                    options.MongoUrl = new MongoUrlBuilder
                    {
                        AllowInsecureTls = options.MongoClientSettings.AllowInsecureTls,
                        ApplicationName = options.MongoClientSettings.ApplicationName,
                        AuthenticationMechanism = options.MongoClientSettings.Credential?.Mechanism,
                        AuthenticationSource = options.MongoClientSettings.Credential?.Source,
                        Compressors = options.MongoClientSettings.Compressors,
                        ConnectTimeout = options.MongoClientSettings.ConnectTimeout,
                        ConnectionMode = options.MongoClientSettings.ConnectionMode,
                        DatabaseName = options.DatabaseName,
                        FSync = options.MongoClientSettings.WriteConcern?.FSync,
                        GuidRepresentation = options.MongoClientSettings.GuidRepresentation,
                        HeartbeatInterval = options.MongoClientSettings.HeartbeatInterval,
                        HeartbeatTimeout = options.MongoClientSettings.HeartbeatTimeout,
                        IPv6 = options.MongoClientSettings.IPv6,
                        Journal = options.MongoClientSettings.WriteConcern?.Journal,
                        LocalThreshold = options.MongoClientSettings.LocalThreshold,
                        MaxConnectionIdleTime = options.MongoClientSettings.MaxConnectionIdleTime,
                        MaxConnectionLifeTime = options.MongoClientSettings.MaxConnectionLifeTime,
                        MaxConnectionPoolSize = options.MongoClientSettings.MaxConnectionPoolSize,
                        MinConnectionPoolSize = options.MongoClientSettings.MinConnectionPoolSize,
                        Password = options.Password,
                        ReadConcernLevel = options.MongoClientSettings.ReadConcern?.Level,
                        ReadPreference = options.MongoClientSettings.ReadPreference,
                        ReplicaSetName = options.MongoClientSettings.ReplicaSetName,
                        RetryReads = options.MongoClientSettings.RetryReads,
                        RetryWrites = options.MongoClientSettings.RetryWrites,
                        Scheme = options.MongoClientSettings.Scheme,
                        Servers = options.MongoClientSettings.Servers,
                        Server = options.MongoClientSettings.Server,
                        ServerSelectionTimeout = options.MongoClientSettings.ServerSelectionTimeout,
                        SocketTimeout = options.MongoClientSettings.SocketTimeout,
                        Username = options.Username,
                        UseTls = options.MongoClientSettings.UseTls,
                        W = options.MongoClientSettings.WriteConcern?.W,
                        WTimeout = options.MongoClientSettings.WriteConcern?.WTimeout,
                        WaitQueueTimeout = options.MongoClientSettings.WaitQueueTimeout,
                    }.ToMongoUrl();
                }

                if (IsNullOrWhiteSpace(options.ConnectionString))
                {
                    options.ConnectionString = options.MongoUrl.ToString();
                }
            }
            else
            {
                if (!IsNullOrWhiteSpace(options.ConnectionString))
                {
                    options.MongoClientSettings = FromConnectionString(options.ConnectionString);
                    if (options.MongoUrl == default)
                    {
                        options.MongoUrl = Create(options.ConnectionString);
                    }
                }
                else
                {
                    if (options.MongoUrl == default)
                    {
                        return;
                    }

                    options.MongoClientSettings = FromUrl(options.MongoUrl);
                    options.ConnectionString = options.MongoUrl.ToString();
                }
            }
        }
    }
}
