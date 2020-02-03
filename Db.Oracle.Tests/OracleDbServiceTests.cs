namespace Services.Db.Oracle.Tests
{
    using System;
    using global::Oracle.ManagedDataAccess.Client;
    using Xunit;

    public class OracleDbServiceTests
    {
        private readonly OracleConnectionStringBuilder _oracleConnectionStringBuilder;

        public OracleDbServiceTests()
        {
            _oracleConnectionStringBuilder = new OracleConnectionStringBuilder();
        }

        [Fact]
        public void ThrowsForInvalidOptions()
        {
            // Arrange
            OracleDbService TestCode() => new OracleDbService(default);

            // Assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        [Fact]
        public void Name()
        {
            // Arrange
            const string name = "TestName";
            var oracleDbService = new OracleDbService(_oracleConnectionStringBuilder)
            {
                Name = name
            };

            // Assert
            Assert.Equal(name, oracleDbService.Name);
        }

        [Fact]
        public void CanCreateDataSourceEnumerator()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleConnectionStringBuilder);

            // Assert
            Assert.True(oracleDbService.CanCreateDataSourceEnumerator);
        }

        [Fact]
        public void CreateCommand()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleConnectionStringBuilder);

            // Act
            var command = oracleDbService.CreateCommand();

            // Assert
            Assert.IsType<OracleCommand>(command);
        }

        [Fact]
        public void CreateCommandBuilder()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleConnectionStringBuilder);

            // Act
            var commandBuilder = oracleDbService.CreateCommandBuilder();

            // Assert
            Assert.IsType<OracleCommandBuilder>(commandBuilder);
        }

        [Fact]
        public void CreateConnection()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleConnectionStringBuilder);

            // Act
            var connection = oracleDbService.CreateConnection();

            // Assert
            Assert.IsType<OracleConnection>(connection);
        }

        [Fact]
        public void CreateConnectionStringBuilder()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleConnectionStringBuilder);

            // Act
            var connectionStringBuilder = oracleDbService.CreateConnectionStringBuilder();

            // Assert
            Assert.IsType<OracleConnectionStringBuilder>(connectionStringBuilder);
        }

        [Fact]
        public void CreateDataAdapter()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleConnectionStringBuilder);

            // Act
            var dataAdapter = oracleDbService.CreateDataAdapter();

            // Assert
            Assert.IsType<OracleDataAdapter>(dataAdapter);
        }

        [Fact]
        public void CreateParameter()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleConnectionStringBuilder);

            // Act
            var parameter = oracleDbService.CreateParameter();

            // Assert
            Assert.IsType<OracleParameter>(parameter);
        }

        [Fact]
        public void CreateDataSourceEnumerator()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleConnectionStringBuilder);

            // Act
            var dataSourceEnumerator = oracleDbService.CreateDataSourceEnumerator();

            // Assert
            Assert.IsType<OracleDataSourceEnumerator>(dataSourceEnumerator);
        }
    }
}
