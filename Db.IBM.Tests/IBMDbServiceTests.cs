namespace Services.Db.IBM.Tests
{
    using System;
    using global::IBM.Data.DB2.Core;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class IBMDbServiceTests
    {
        private readonly Mock<IOptions<IBMDbOptions>> _ibmDbOptions;

        public IBMDbServiceTests()
        {
            _ibmDbOptions = new Mock<IOptions<IBMDbOptions>>();
            _ibmDbOptions.Setup(x => x.Value).Returns(new IBMDbOptions
            {
                Database = nameof(IBMDbOptions.Database),
                DBName = nameof(IBMDbOptions.DBName),
                UserId = nameof(IBMDbOptions.UserId),
                Password = nameof(IBMDbOptions.Password)
            });
        }

        [Fact]
        public void ThrowsForInvalidOptions()
        {
            // Arrange
            static object TestCode() => new IBMDbService(default);

            // Assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        [Fact]
        public void Provider()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_ibmDbOptions.Object);

            // Assert
            Assert.Equal("IBM", ibmDbService.Provider);
        }

        [Fact]
        public void CanCreateDataSourceEnumerator()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_ibmDbOptions.Object);

            // Assert
            Assert.True(ibmDbService.CanCreateDataSourceEnumerator);
        }

        [Fact]
        public void CreateCommand()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_ibmDbOptions.Object);

            // Act
            var command = ibmDbService.CreateCommand();

            // Assert
            Assert.IsType<DB2Command>(command);
        }

        [Fact]
        public void CreateCommandBuilder()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_ibmDbOptions.Object);

            // Act
            var commandBuilder = ibmDbService.CreateCommandBuilder();

            // Assert
            Assert.IsType<DB2CommandBuilder>(commandBuilder);
        }

        [Fact]
        public void CreateConnection()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_ibmDbOptions.Object);

            // Act
            var connection = ibmDbService.CreateConnection();

            // Assert
            Assert.IsType<DB2Connection>(connection);
        }

        [Fact]
        public void CreateConnectionStringBuilder()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_ibmDbOptions.Object);

            // Act
            var connectionStringBuilder = ibmDbService.CreateConnectionStringBuilder();

            // Assert
            var ibmConnectionStringBuilder = Assert.IsType<DB2ConnectionStringBuilder>(connectionStringBuilder);
            Assert.Equal(_ibmDbOptions.Object.Value.Database, ibmConnectionStringBuilder.Database);
            Assert.Equal(_ibmDbOptions.Object.Value.DBName, ibmConnectionStringBuilder.DBName);
            Assert.Equal(_ibmDbOptions.Object.Value.UserId, ibmConnectionStringBuilder.UserID);
            Assert.Equal(_ibmDbOptions.Object.Value.Password, ibmConnectionStringBuilder.Password);
        }

        [Fact]
        public void CreateDataAdapter()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_ibmDbOptions.Object);

            // Act
            var dataAdapter = ibmDbService.CreateDataAdapter();

            // Assert
            Assert.IsType<DB2DataAdapter>(dataAdapter);
        }

        [Fact]
        public void CreateParameter()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_ibmDbOptions.Object);

            // Act
            var parameter = ibmDbService.CreateParameter();

            // Assert
            Assert.IsType<DB2Parameter>(parameter);
        }

        [Fact]
        public void CreateDataSourceEnumerator()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_ibmDbOptions.Object);

            // Act
            var dataSourceEnumerator = ibmDbService.CreateDataSourceEnumerator();

            // Assert
            Assert.IsType<DB2DataSourceEnumerator>(dataSourceEnumerator);
        }
    }
}
