namespace Services.Db.IBM.Tests
{
    using System;
    using global::IBM.Data.DB2.Core;
    using Xunit;

    public class IBMDbServiceTests
    {
        private readonly DB2ConnectionStringBuilder _db2ConnectionStringBuilder;

        public IBMDbServiceTests()
        {
            _db2ConnectionStringBuilder = new DB2ConnectionStringBuilder();
        }

        [Fact]
        public void ThrowsForInvalidOptions()
        {
            // Arrange
            IBMDbService TestCode() => new IBMDbService(default);

            // Assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        [Fact]
        public void Name()
        {
            // Arrange
            const string name = "TestName";
            var ibmDbService = new IBMDbService(_db2ConnectionStringBuilder)
            {
                Name = name
            };

            // Assert
            Assert.Equal(name, ibmDbService.Name);
        }

        [Fact]
        public void CanCreateDataSourceEnumerator()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_db2ConnectionStringBuilder);

            // Assert
            Assert.True(ibmDbService.CanCreateDataSourceEnumerator);
        }

        [Fact]
        public void CreateCommand()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_db2ConnectionStringBuilder);

            // Act
            var command = ibmDbService.CreateCommand();

            // Assert
            Assert.IsType<DB2Command>(command);
        }

        [Fact]
        public void CreateCommandBuilder()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_db2ConnectionStringBuilder);

            // Act
            var commandBuilder = ibmDbService.CreateCommandBuilder();

            // Assert
            Assert.IsType<DB2CommandBuilder>(commandBuilder);
        }

        [Fact]
        public void CreateConnection()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_db2ConnectionStringBuilder);

            // Act
            var connection = ibmDbService.CreateConnection();

            // Assert
            Assert.IsType<DB2Connection>(connection);
        }

        [Fact]
        public void CreateConnectionStringBuilder()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_db2ConnectionStringBuilder);

            // Act
            var connectionStringBuilder = ibmDbService.CreateConnectionStringBuilder();

            // Assert
            Assert.IsType<DB2ConnectionStringBuilder>(connectionStringBuilder);
        }

        [Fact]
        public void CreateDataAdapter()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_db2ConnectionStringBuilder);

            // Act
            var dataAdapter = ibmDbService.CreateDataAdapter();

            // Assert
            Assert.IsType<DB2DataAdapter>(dataAdapter);
        }

        [Fact]
        public void CreateParameter()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_db2ConnectionStringBuilder);

            // Act
            var parameter = ibmDbService.CreateParameter();

            // Assert
            Assert.IsType<DB2Parameter>(parameter);
        }

        [Fact]
        public void CreateDataSourceEnumerator()
        {
            // Arrange
            var ibmDbService = new IBMDbService(_db2ConnectionStringBuilder);

            // Act
            var dataSourceEnumerator = ibmDbService.CreateDataSourceEnumerator();

            // Assert
            Assert.IsType<DB2DataSourceEnumerator>(dataSourceEnumerator);
        }
    }
}
