namespace Services
{
    using System;
    using System.Data.Common;
    using Common.Services;
    using JetBrains.Annotations;
    using Teradata.Client.Provider;

    /// <inheritdoc cref="IDbService" />
    [PublicAPI]
    public class TeradataDbService : DbProviderFactory, IDbService
    {
        private readonly TdConnectionStringBuilder _connectionStringBuilder;

        /// <summary>Initializes a new instance of the <see cref="TeradataDbService"/> class.</summary>
        /// <param name="connectionStringBuilder">The connection string builder.</param>
        /// <exception cref="ArgumentNullException"><paramref name="connectionStringBuilder"/> is <see langword="null"/>.</exception>
        public TeradataDbService(TdConnectionStringBuilder connectionStringBuilder)
        {
            _connectionStringBuilder = connectionStringBuilder ?? throw new ArgumentNullException(nameof(connectionStringBuilder));
        }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public bool CanCreateDataAdapter => true;

        /// <inheritdoc />
        public bool CanCreateCommandBuilder => true;

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
