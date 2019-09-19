namespace Services
{
    using System.Data;
    using System.Data.SqlClient;
    using Common;
    using Microsoft.Extensions.Options;

    public class SqlDbService : IDbService
    {
        private readonly SqlConnectionStringBuilder _connectionStringBuilder;
        private IDbConnection _dbConnection;

        public string Provider => "Sql";

        public SqlDbService(IOptions<SqlDbOptions> sqlDbOptions)
        {
            _connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = sqlDbOptions.Value.DataSource,
                UserID = sqlDbOptions.Value.UserId,
                Password = sqlDbOptions.Value.Password
            };
        }

        public IDbConnection GetConnection()
        {
            return _dbConnection ?? (_dbConnection = new SqlConnection(_connectionStringBuilder.ConnectionString));
        }

        public IDbDataAdapter GetDataAdapter(string commandText)
        {
            return new SqlDataAdapter(commandText, _connectionStringBuilder.ConnectionString);
        }
    }
}
