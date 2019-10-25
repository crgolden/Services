namespace Services
{
    using System;
    using System.Data.Common;
    using System.Diagnostics.CodeAnalysis;
    using Common;
    using IBM.Data.DB2.Core;
    using Microsoft.Extensions.Options;
    using static Common.DbServiceType;

    /// <inheritdoc cref="IDbService" />
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "IBM is an abbreviation")]
    public class IBMDbService : DbProviderFactory, IDbService
    {
        private readonly DB2ConnectionStringBuilder _connectionStringBuilder;

        public IBMDbService(IOptions<IBMDbOptions>? ibmDbOptions)
        {
            if (ibmDbOptions?.Value == default)
            {
                throw new ArgumentNullException(nameof(ibmDbOptions));
            }

            _connectionStringBuilder = new DB2ConnectionStringBuilder
            {
                Database = ibmDbOptions.Value.Database,
                DBName = ibmDbOptions.Value.DBName,
                UserID = ibmDbOptions.Value.UserId,
                Password = ibmDbOptions.Value.Password
            };
        }

        /// <inheritdoc />
        public string? Name { get; set; }

        /// <inheritdoc />
        public DbServiceType Type => IBM;

        /// <inheritdoc cref="IDbService" />
        public override bool CanCreateDataSourceEnumerator => true;

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
