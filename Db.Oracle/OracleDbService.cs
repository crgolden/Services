namespace Services
{
    using System;
    using System.Data.Common;
    using Common.Services;
    using JetBrains.Annotations;
    using Oracle.ManagedDataAccess.Client;

    /// <inheritdoc cref="IDbService" />
    [PublicAPI]
    public class OracleDbService : DbProviderFactory, IDbService
    {
        private readonly OracleConnectionStringBuilder _connectionStringBuilder;

        /// <summary>Initializes a new instance of the <see cref="OracleDbService"/> class.</summary>
        /// <param name="oracleConnectionStringBuilder">The oracle connection string builder.</param>
        /// <exception cref="ArgumentNullException"><paramref name="oracleConnectionStringBuilder"/> is <see langword="null"/>.</exception>
        public OracleDbService(OracleConnectionStringBuilder oracleConnectionStringBuilder)
        {
            _connectionStringBuilder = oracleConnectionStringBuilder ?? throw new ArgumentNullException(nameof(oracleConnectionStringBuilder));
        }

        /// <inheritdoc />
        public string Name { get; set; }

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
