namespace Services
{
    using System;
    using System.Data.Common;
    using System.Diagnostics.CodeAnalysis;
    using Common;
    using IBM.Data.DB2.Core;
    using JetBrains.Annotations;

    /// <inheritdoc cref="IDbService" />
    [PublicAPI]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "IBM is an abbreviation")]
    public class IBMDbService : DbProviderFactory, IDbService
    {
        private readonly DB2ConnectionStringBuilder _connectionStringBuilder;

        /// <summary>Initializes a new instance of the <see cref="IBMDbService"/> class.</summary>
        /// <param name="db2ConnectionStringBuilder">The DB2 connection string builder.</param>
        /// <exception cref="ArgumentNullException"><paramref name="db2ConnectionStringBuilder"/> is <see langword="null"/>.</exception>
        public IBMDbService(DB2ConnectionStringBuilder db2ConnectionStringBuilder)
        {
            _connectionStringBuilder = db2ConnectionStringBuilder ?? throw new ArgumentNullException(nameof(db2ConnectionStringBuilder));
        }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc cref="IDbService" />
        public override bool CanCreateDataSourceEnumerator => true;

        /// <inheritdoc />
        public bool CanCreateDataAdapter => true;

        /// <inheritdoc />
        public bool CanCreateCommandBuilder => true;

        /// <inheritdoc cref="IDbService" />
        public override DbCommand CreateCommand() => new DB2Command();

        /// <inheritdoc cref="IDbService" />
        public override DbCommandBuilder CreateCommandBuilder() => new DB2CommandBuilder();

        /// <inheritdoc cref="IDbService" />
        public override DbConnection CreateConnection() => new DB2Connection(_connectionStringBuilder.ConnectionString);

        /// <inheritdoc cref="IDbService" />
        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => _connectionStringBuilder;

        /// <inheritdoc cref="IDbService" />
        public override DbDataAdapter CreateDataAdapter() => new DB2DataAdapter();

        /// <inheritdoc cref="IDbService" />
        public override DbParameter CreateParameter() => new DB2Parameter();

        /// <inheritdoc cref="IDbService" />
        public override DbDataSourceEnumerator CreateDataSourceEnumerator() => new DB2DataSourceEnumerator();
    }
}
