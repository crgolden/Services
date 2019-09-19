namespace Services
{
    using System.Data;
    using Common;
    using Teradata.Client.Provider;
    using Microsoft.Extensions.Options;

    public class TeradataDbService : IDbService
    {
        private readonly TdConnectionStringBuilder _connectionStringBuilder;
        private IDbConnection _dbConnection;

        public string Provider => "Teradata";

        public TeradataDbService(IOptions<TeradataDbOptions> teradataDbOptions)
        {
            _connectionStringBuilder = new TdConnectionStringBuilder
            {
                DataSource = teradataDbOptions.Value.DataSource,
                Database = teradataDbOptions.Value.Database,
                UserId = teradataDbOptions.Value.UserId,
                Password = teradataDbOptions.Value.Password,
                AuthenticationMechanism = teradataDbOptions.Value.AuthenticationMechanism
            };
        }

        public IDbConnection GetConnection()
        {
            return _dbConnection ?? (_dbConnection = new TdConnection(_connectionStringBuilder.ConnectionString));
        }

        public IDbDataAdapter GetDataAdapter(string commandText)
        {
            return new TdDataAdapter(commandText, _connectionStringBuilder.ConnectionString);
        }
    }
}
