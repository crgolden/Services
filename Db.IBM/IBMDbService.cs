namespace Services
{
    using System;
    using System.Data.Common;
    using Common;
    using IBM.Data.DB2.Core;
    using Microsoft.Extensions.Options;
    using static Common.DbServiceType;

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

        public string? Name { get; set; }

        public DbServiceType Type => IBM;

        public override bool CanCreateDataSourceEnumerator => true;

        public override DbCommand CreateCommand() => new DB2Command();

        public override DbCommandBuilder CreateCommandBuilder() => new DB2CommandBuilder();

        public override DbConnection CreateConnection() => new DB2Connection(_connectionStringBuilder.ConnectionString);

        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => _connectionStringBuilder;

        public override DbDataAdapter CreateDataAdapter() => new DB2DataAdapter();

        public override DbParameter CreateParameter() => new DB2Parameter();

        public override DbDataSourceEnumerator CreateDataSourceEnumerator() => new DB2DataSourceEnumerator();
    }
}