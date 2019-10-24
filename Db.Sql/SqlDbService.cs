namespace Services
{
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;
    using Common;
    using Microsoft.Extensions.Options;
    using static Common.DbServiceType;

    /// <inheritdoc cref="IDbService" />
    public class SqlDbService : DbProviderFactory, IDbService
    {
        private readonly SqlConnectionStringBuilder _connectionStringBuilder;

        public SqlDbService(IOptions<SqlDbOptions>? sqlDbOptions)
        {
            if (sqlDbOptions?.Value == default)
            {
                throw new ArgumentNullException(nameof(sqlDbOptions));
            }

            _connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = sqlDbOptions.Value.DataSource,
                UserID = sqlDbOptions.Value.UserId,
                Password = sqlDbOptions.Value.Password
            };
        }

        /// <inheritdoc />
        public string? Name { get; set; }

        /// <inheritdoc />
        public DbServiceType Type => Sql;

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
