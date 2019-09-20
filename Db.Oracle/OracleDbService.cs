namespace Services
{
    using System;
    using System.Data.Common;
    using Common;
    using Microsoft.Extensions.Options;
    using Oracle.ManagedDataAccess.Client;

    public class OracleDbService : DbProviderFactory, IDbService
    {
        private readonly OracleConnectionStringBuilder _connectionStringBuilder;

        public OracleDbService(IOptions<OracleDbOptions> oracleDbOptions)
        {
            if (oracleDbOptions.Value == default)
            {
                throw new ArgumentNullException(nameof(OracleDbOptions));
            }

            _connectionStringBuilder = new OracleConnectionStringBuilder
            {
                DataSource = oracleDbOptions.Value.DataSource,
                UserID = oracleDbOptions.Value.UserId,
                Password = oracleDbOptions.Value.Password
            };
        }

        public string Provider => "Oracle";

        public override bool CanCreateDataSourceEnumerator => true;

        public override DbCommand CreateCommand() => new OracleCommand();

        public override DbCommandBuilder CreateCommandBuilder() => new OracleCommandBuilder();

        public override DbConnection CreateConnection() => new OracleConnection(_connectionStringBuilder.ConnectionString);

        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => _connectionStringBuilder;

        public override DbDataAdapter CreateDataAdapter() => new OracleDataAdapter();

        public override DbParameter CreateParameter() => new OracleParameter();

        public override DbDataSourceEnumerator CreateDataSourceEnumerator() => new OracleDataSourceEnumerator();
    }
}
