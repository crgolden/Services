namespace Services
{
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;
    using Common;
    using Microsoft.Extensions.Options;

    public class SqlDbService : DbProviderFactory, IDbService
    {
        private readonly SqlConnectionStringBuilder _connectionStringBuilder;

        public SqlDbService(IOptions<SqlDbOptions> sqlDbOptions)
        {
            if (sqlDbOptions.Value == default)
            {
                throw new ArgumentNullException(nameof(SqlDbOptions));
            }

            _connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = sqlDbOptions.Value.DataSource,
                UserID = sqlDbOptions.Value.UserId,
                Password = sqlDbOptions.Value.Password
            };
        }

        public string Provider => "Sql";

        public override DbCommand CreateCommand() => new SqlCommand();

        public override DbCommandBuilder CreateCommandBuilder() => new SqlCommandBuilder();

        public override DbConnection CreateConnection() => new SqlConnection(_connectionStringBuilder.ConnectionString);

        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => _connectionStringBuilder;

        public override DbDataAdapter CreateDataAdapter() => new SqlDataAdapter();

        public override DbParameter CreateParameter() => new SqlParameter();
    }
}
