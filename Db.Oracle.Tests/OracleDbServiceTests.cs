namespace Services.Db.Oracle.Tests
{
    using System;
    using global::Oracle.ManagedDataAccess.Client;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;
    using static Common.DbServiceType;
    using static Xunit.Assert;

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
            Throws<ArgumentNullException>(TestCode);
        }

        [Fact]
        public void Provider()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Assert
            Equal(Oracle, oracleDbService.Type);
        }

        [Fact]
        public void Name()
        {
            // Arrange
            const string name = "TestName";
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object)
            {
                Name = name
            };

            // Assert
            Equal(name, oracleDbService.Name);
        }

        [Fact]
        public void CanCreateDataSourceEnumerator()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Assert
            True(oracleDbService.CanCreateDataSourceEnumerator);
        }

        [Fact]
        public void CreateCommand()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Act
            var command = oracleDbService.CreateCommand();

            // Assert
            IsType<OracleCommand>(command);
        }

        [Fact]
        public void CreateCommandBuilder()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Act
            var commandBuilder = oracleDbService.CreateCommandBuilder();

            // Assert
            IsType<OracleCommandBuilder>(commandBuilder);
        }

        [Fact]
        public void CreateConnection()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Act
            var connection = oracleDbService.CreateConnection();

            // Assert
            IsType<OracleConnection>(connection);
        }

        [Fact]
        public void CreateConnectionStringBuilder()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Act
            var connectionStringBuilder = oracleDbService.CreateConnectionStringBuilder();

            // Assert
            var oracleConnectionStringBuilder = IsType<OracleConnectionStringBuilder>(connectionStringBuilder);
            Equal(_oracleDbOptions.Object.Value.DataSource, oracleConnectionStringBuilder.DataSource);
            Equal(_oracleDbOptions.Object.Value.UserId, oracleConnectionStringBuilder.UserID);
            Equal(_oracleDbOptions.Object.Value.Password, oracleConnectionStringBuilder.Password);
        }

        [Fact]
        public void CreateDataAdapter()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Act
            var dataAdapter = oracleDbService.CreateDataAdapter();

            // Assert
            IsType<OracleDataAdapter>(dataAdapter);
        }

        [Fact]
        public void CreateParameter()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Act
            var parameter = oracleDbService.CreateParameter();

            // Assert
            IsType<OracleParameter>(parameter);
        }

        [Fact]
        public void CreateDataSourceEnumerator()
        {
            // Arrange
            var oracleDbService = new OracleDbService(_oracleDbOptions.Object);

            // Act
            var dataSourceEnumerator = oracleDbService.CreateDataSourceEnumerator();

            // Assert
            IsType<OracleDataSourceEnumerator>(dataSourceEnumerator);
        }
    }
}
