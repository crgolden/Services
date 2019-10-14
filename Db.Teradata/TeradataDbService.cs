namespace Services
{
    using System;
    using System.Data.Common;
    using Common;
    using Microsoft.Extensions.Options;
    using Teradata.Client.Provider;

    public class TeradataDbService : DbProviderFactory, IDbService
    {
        private readonly TdConnectionStringBuilder _connectionStringBuilder;

        public TeradataDbService(IOptions<TeradataDbOptions> teradataDbOptions)
        {
            if (teradataDbOptions?.Value == default)
            {
                throw new ArgumentNullException(nameof(teradataDbOptions));
            }

            _connectionStringBuilder = new TdConnectionStringBuilder
            {
                DataSource = teradataDbOptions.Value.DataSource,
                Database = teradataDbOptions.Value.Database,
                UserId = teradataDbOptions.Value.UserId,
                Password = teradataDbOptions.Value.Password,
                AuthenticationMechanism = teradataDbOptions.Value.AuthenticationMechanism
            };
        }

        public string Provider => "Teradata";

        public override DbCommand CreateCommand() => new TdCommand();

        public override DbCommandBuilder CreateCommandBuilder() => new TdCommandBuilder();

        public override DbConnection CreateConnection() => new TdConnection(_connectionStringBuilder.ConnectionString);

        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => _connectionStringBuilder;

        public override DbDataAdapter CreateDataAdapter() => new TdDataAdapter();

        public override DbParameter CreateParameter() => new TdParameter();
    }
}
