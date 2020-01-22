namespace Services.Options
{
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;
    using static System.String;
    using static MongoDB.Driver.MongoClientSettings;
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
#pragma warning disable CS0618 // Type or member is obsolete
                        Password = options.MongoClientSettings.Credential?.Password,
#pragma warning restore CS0618 // Type or member is obsolete
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
                        Username = options.MongoClientSettings.Credential?.Username,
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
