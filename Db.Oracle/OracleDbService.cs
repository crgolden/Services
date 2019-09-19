namespace Services
{
    using System.Data;
    using Common;
    using Oracle.ManagedDataAccess.Client;
    using Microsoft.Extensions.Options;

    public class OracleDbService : IDbService
    {
        private readonly OracleConnectionStringBuilder _connectionStringBuilder;
        private IDbConnection _dbConnection;

        public string Provider => "Oracle";

        public OracleDbService(IOptions<OracleDbOptions> oracleDbOptions)
        {
            _connectionStringBuilder = new OracleConnectionStringBuilder
            {
                DataSource = oracleDbOptions.Value.DataSource,
                UserID = oracleDbOptions.Value.UserId,
                Password = oracleDbOptions.Value.Password
            };
        }

        public IDbConnection GetConnection()
        {
            return _dbConnection ?? (_dbConnection = new OracleConnection(_connectionStringBuilder.ConnectionString));
        }

        public IDbDataAdapter GetDataAdapter(string selectCommandText)
        {
            return new OracleDataAdapter(selectCommandText, _connectionStringBuilder.ConnectionString);
        }
    }
}
