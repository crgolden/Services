namespace Services
{
    using System;
    using System.Data.Common;
    using Common;
    using Microsoft.Extensions.Options;
    using Oracle.ManagedDataAccess.Client;
    using static Common.DbServiceType;

    /// <inheritdoc cref="IDbService" />
    public class OracleDbService : DbProviderFactory, IDbService
    {
        private readonly OracleConnectionStringBuilder _connectionStringBuilder;

        public OracleDbService(IOptions<OracleDbOptions>? oracleDbOptions)
        {
            if (oracleDbOptions?.Value == default)
            {
                throw new ArgumentNullException(nameof(oracleDbOptions));
            }

            _connectionStringBuilder = new OracleConnectionStringBuilder
            {
                DataSource = oracleDbOptions.Value.DataSource,
                UserID = oracleDbOptions.Value.UserId,
                Password = oracleDbOptions.Value.Password
            };
        }

        /// <inheritdoc />
        public string? Name { get; set; }

        /// <inheritdoc />
        public DbServiceType Type => Oracle;

        /// <inheritdoc cref="IDbService" />
        public override bool CanCreateDataSourceEnumerator => true;

        /// <inheritdoc cref="IDbService" />
        public override DbCommand CreateCommand() => new OracleCommand();

        /// <inheritdoc cref="IDbService" />
        public override DbCommandBuilder CreateCommandBuilder() => new OracleCommandBuilder();

        /// <inheritdoc cref="IDbService" />
        public override DbConnection CreateConnection() => new OracleConnection(_connectionStringBuilder.ConnectionString);

        /// <inheritdoc cref="IDbService" />
        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => _connectionStringBuilder;

        /// <inheritdoc cref="IDbService" />
        public override DbDataAdapter CreateDataAdapter() => new OracleDataAdapter();

        /// <inheritdoc cref="IDbService" />
        public override DbParameter CreateParameter() => new OracleParameter();

        /// <inheritdoc cref="IDbService" />
        public override DbDataSourceEnumerator CreateDataSourceEnumerator() => new OracleDataSourceEnumerator();
    }
}
