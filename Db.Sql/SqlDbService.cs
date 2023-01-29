namespace Services
{
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;
    using Common.Services;
    using JetBrains.Annotations;

    /// <inheritdoc cref="IDbService" />
    [PublicAPI]
    public class SqlDbService : DbProviderFactory, IDbService
    {
        private readonly SqlConnectionStringBuilder _connectionStringBuilder;

        /// <summary>Initializes a new instance of the <see cref="SqlDbService"/> class.</summary>
        /// <param name="sqlConnectionStringBuilder">The SQL connection string builder.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sqlConnectionStringBuilder"/>is <see langword="null"/>.</exception>
        public SqlDbService(SqlConnectionStringBuilder sqlConnectionStringBuilder)
        {
            _connectionStringBuilder = sqlConnectionStringBuilder ?? throw new ArgumentNullException(nameof(sqlConnectionStringBuilder));
        }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public bool CanCreateDataAdapter => true;

        /// <inheritdoc />
        public bool CanCreateCommandBuilder => true;

        /// <inheritdoc cref="IDbService" />
        public override DbCommand CreateCommand() => new SqlCommand();

        /// <inheritdoc cref="IDbService" />
        public override DbCommandBuilder CreateCommandBuilder() => new SqlCommandBuilder();

        /// <inheritdoc cref="IDbService" />
        public override DbConnection CreateConnection() => new SqlConnection(_connectionStringBuilder.ConnectionString);

        /// <inheritdoc cref="IDbService" />
        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => _connectionStringBuilder;

        /// <inheritdoc cref="IDbService" />
        public override DbDataAdapter CreateDataAdapter() => new SqlDataAdapter();

        /// <inheritdoc cref="IDbService" />
        public override DbParameter CreateParameter() => new SqlParameter();
    }
}
