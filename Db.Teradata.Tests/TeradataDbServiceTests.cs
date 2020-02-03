namespace Services.Db.Teradata.Tests
{
    using System;
    using global::Teradata.Client.Provider;
    using Xunit;

    public class TeradataDbServiceTests
    {
        private readonly TdConnectionStringBuilder _tdConnectionStringBuilder;

        public TeradataDbServiceTests()
        {
            _tdConnectionStringBuilder = new TdConnectionStringBuilder();
        }

        [Fact]
        public void ThrowsForInvalidOptions()
        {
            // Arrange
            TeradataDbService TestCode() => new TeradataDbService(default);

            // Assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        [Fact]
        public void Name()
        {
            // Arrange
            const string name = "TestName";
            var teradataDbService = new TeradataDbService(_tdConnectionStringBuilder)
            {
                Name = name
            };

            // Assert
            Assert.Equal(name, teradataDbService.Name);
        }

        [Fact]
        public void CreateCommand()
        {
            // Arrange
            var teradataDbService = new TeradataDbService(_tdConnectionStringBuilder);

            // Act
            var command = teradataDbService.CreateCommand();

            // Assert
            Assert.IsType<TdCommand>(command);
        }

        [Fact]
        public void CreateCommandBuilder()
        {
            // Arrange
            var teradataDbService = new TeradataDbService(_tdConnectionStringBuilder);

            // Act
            var commandBuilder = teradataDbService.CreateCommandBuilder();

            // Assert
            Assert.IsType<TdCommandBuilder>(commandBuilder);
        }

        [Fact]
        public void CreateConnection()
        {
            // Arrange
            var teradataDbService = new TeradataDbService(_tdConnectionStringBuilder);

            // Act
            var connection = teradataDbService.CreateConnection();

            // Assert
            Assert.IsType<TdConnection>(connection);
        }

        [Fact]
        public void CreateConnectionStringBuilder()
        {
            // Arrange
            var teradataDbService = new TeradataDbService(_tdConnectionStringBuilder);

            // Act
            var connectionStringBuilder = teradataDbService.CreateConnectionStringBuilder();

            // Assert
            Assert.IsType<TdConnectionStringBuilder>(connectionStringBuilder);
        }

        [Fact]
        public void CreateDataAdapter()
        {
            // Arrange
            var teradataDbService = new TeradataDbService(_tdConnectionStringBuilder);

            // Act
            var dataAdapter = teradataDbService.CreateDataAdapter();

            // Assert
            Assert.IsType<TdDataAdapter>(dataAdapter);
        }

        [Fact]
        public void CreateParameter()
        {
            // Arrange
            var teradataDbService = new TeradataDbService(_tdConnectionStringBuilder);

            // Act
            var parameter = teradataDbService.CreateParameter();

            // Assert
            Assert.IsType<TdParameter>(parameter);
        }
    }
}
