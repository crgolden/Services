namespace Services
{
    using System;
    using System.Data.Common;
    using Common;
    using Microsoft.Extensions.Options;
    using Teradata.Client.Provider;
    using static Common.DbServiceType;

    /// <inheritdoc cref="IDbService" />
    public class TeradataDbService : DbProviderFactory, IDbService
    {
        private readonly TdConnectionStringBuilder _connectionStringBuilder;

        public TeradataDbService(IOptions<TeradataDbOptions>? teradataDbOptions)
        {
            if (teradataDbOptions?.Value == default)
            {
                throw new ArgumentNullException(nameof(teradataDbOptions));
            }

            _connectionStringBuilder = new TdConnectionStringBuilder
            {
                DataSource = teradataDbOptions.Value.DataSource,
                Database = teradataDbOptions.Value.Database,
                UserId = teradataDbOptions.Value.UserId,
                Password = teradataDbOptions.Value.Password,
                AuthenticationMechanism = teradataDbOptions.Value.AuthenticationMechanism
            };
        }

        /// <inheritdoc />
        public string? Name { get; set; }

        /// <inheritdoc />
        public DbServiceType Type => Teradata;

        /// <inheritdoc cref="IDbService" />
        public override DbCommand CreateCommand() => new TdCommand();

        /// <inheritdoc cref="IDbService" />
        public override DbCommandBuilder CreateCommandBuilder() => new TdCommandBuilder();

        /// <inheritdoc cref="IDbService" />
        public override DbConnection CreateConnection() => new TdConnection(_connectionStringBuilder.ConnectionString);

        /// <inheritdoc cref="IDbService" />
        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => _connectionStringBuilder;

        /// <inheritdoc cref="IDbService" />
        public override DbDataAdapter CreateDataAdapter() => new TdDataAdapter();

        /// <inheritdoc cref="IDbService" />
        public override DbParameter CreateParameter() => new TdParameter();
    }
}
