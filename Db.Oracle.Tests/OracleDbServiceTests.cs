namespace Services.Db.Oracle.Tests
{
    using System;
    using global::Oracle.ManagedDataAccess.Client;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class OracleDbServiceTests
    {
        private readonly Mock<IOptions<OracleDbOptions>> _oracleDbOptions;

        public OracleDbServiceTests()
        {
            _oracleDbOptions = new Mock<IOptions<OracleDbOptions>>();
            _oracleDbOptions.Setup(x => x.Value).Returns(new OracleDbOptions
            {
                DataSource = nameof(OracleDbOptions.DataSource),
                UserId = nameof(OracleDbOptions.UserId),
                Password = nameof(OracleDbOptions.Password)
            });
        }

        [Fact]
        public void ThrowsForInvalidOptions()
        {
            // Arrange
            static object TestCode() => new OracleDbService(default);

            // Assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        [Fact]
        public void Provider()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Assert
            Assert.Equal("Oracle", oracleDbService.Provider);
        }

        [Fact]
        public void CanCreateDataSourceEnumerator()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Assert
            Assert.True(oracleDbService.CanCreateDataSourceEnumerator);
        }

        [Fact]
        public void CreateCommand()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Act
            var command = oracleDbService.CreateCommand();

            // Assert
            Assert.IsType<OracleCommand>(command);
        }

        [Fact]
        public void CreateCommandBuilder()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Act
            var commandBuilder = oracleDbService.CreateCommandBuilder();

            // Assert
            Assert.IsType<OracleCommandBuilder>(commandBuilder);
        }

        [Fact]
        public void CreateConnection()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Act
            var connection = oracleDbService.CreateConnection();

            // Assert
            Assert.IsType<OracleConnection>(connection);
        }

        [Fact]
        public void CreateConnectionStringBuilder()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Act
            var connectionStringBuilder = oracleDbService.CreateConnectionStringBuilder();

            // Assert
            var oracleConnectionStringBuilder = Assert.IsType<OracleConnectionStringBuilder>(connectionStringBuilder);
            Assert.Equal(_oracleDbOptions.Object.Value.DataSource, oracleConnectionStringBuilder.DataSource);
            Assert.Equal(_oracleDbOptions.Object.Value.UserId, oracleConnectionStringBuilder.UserID);
            Assert.Equal(_oracleDbOptions.Object.Value.Password, oracleConnectionStringBuilder.Password);
        }

        [Fact]
        public void CreateDataAdapter()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Act
            var dataAdapter = oracleDbService.CreateDataAdapter();

            // Assert
            Assert.IsType<OracleDataAdapter>(dataAdapter);
        }

        [Fact]
        public void CreateParameter()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Act
            var parameter = oracleDbService.CreateParameter();

            // Assert
            Assert.IsType<OracleParameter>(parameter);
        }

        [Fact]
        public void CreateDataSourceEnumerator()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Act
            var dataSourceEnumerator = oracleDbService.CreateDataSourceEnumerator();

            // Assert
            Assert.IsType<OracleDataSourceEnumerator>(dataSourceEnumerator);
        }
    }
}
