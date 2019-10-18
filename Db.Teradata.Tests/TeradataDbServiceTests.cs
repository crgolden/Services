namespace Services.Db.Teradata.Tests
{
    using System;
    using global::Teradata.Client.Provider;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;
    using static Common.DbServiceType;

    public class TeradataDbServiceTests
    {
        private readonly Mock<IOptions<TeradataDbOptions>> _teradataDbOptions;

        public TeradataDbServiceTests()
        {
            _teradataDbOptions = new Mock<IOptions<TeradataDbOptions>>();
            _teradataDbOptions.Setup(x => x.Value).Returns(new TeradataDbOptions
            {
                Database = nameof(TeradataDbOptions.Database),
                DataSource = nameof(TeradataDbOptions.DataSource),
                UserId = nameof(TeradataDbOptions.UserId),
                Password = nameof(TeradataDbOptions.Password),
                AuthenticationMechanism = nameof(TeradataDbOptions.AuthenticationMechanism)
            });
        }

        [Fact]
        public void ThrowsForInvalidOptions()
        {
            // Arrange
            static object TestCode() => new TeradataDbService(default);

            // Assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        [Fact]
        public void Provider()
        {
            // Arrange
            var teradataDbService = new TeradataDbService(_teradataDbOptions.Object);

            // Assert
            Assert.Equal(Teradata, teradataDbService.Type);
        }

        [Fact]
        public void Name()
        {
            // Arrange
            const string name = "TestName";
            var teradataDbService = new TeradataDbService(_teradataDbOptions.Object)
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
            var teradataDbService = new TeradataDbService(_teradataDbOptions.Object);

            // Act
            var command = teradataDbService.CreateCommand();

            // Assert
            Assert.IsType<TdCommand>(command);
        }

        [Fact]
        public void CreateCommandBuilder()
        {
            // Arrange
            var teradataDbService = new TeradataDbService(_teradataDbOptions.Object);

            // Act
            var commandBuilder = teradataDbService.CreateCommandBuilder();

            // Assert
            Assert.IsType<TdCommandBuilder>(commandBuilder);
        }

        [Fact]
        public void CreateConnection()
        {
            // Arrange
            var teradataDbService = new TeradataDbService(_teradataDbOptions.Object);

            // Act
            var connection = teradataDbService.CreateConnection();

            // Assert
            Assert.IsType<TdConnection>(connection);
        }

        [Fact]
        public void CreateConnectionStringBuilder()
        {
            // Arrange
            var teradataDbService = new TeradataDbService(_teradataDbOptions.Object);

            // Act
            var connectionStringBuilder = teradataDbService.CreateConnectionStringBuilder();

            // Assert
            var teradataConnectionStringBuilder = Assert.IsType<TdConnectionStringBuilder>(connectionStringBuilder);
            Assert.Equal(_teradataDbOptions.Object.Value.Database, teradataConnectionStringBuilder.Database);
            Assert.Equal(_teradataDbOptions.Object.Value.DataSource, teradataConnectionStringBuilder.DataSource);
            Assert.Equal(_teradataDbOptions.Object.Value.UserId, teradataConnectionStringBuilder.UserId);
            Assert.Equal(_teradataDbOptions.Object.Value.Password, teradataConnectionStringBuilder.Password);
            Assert.Equal(_teradataDbOptions.Object.Value.AuthenticationMechanism, teradataConnectionStringBuilder.AuthenticationMechanism);
        }

        [Fact]
        public void CreateDataAdapter()
        {
            // Arrange
            var teradataDbService = new TeradataDbService(_teradataDbOptions.Object);

            // Act
            var dataAdapter = teradataDbService.CreateDataAdapter();

            // Assert
            Assert.IsType<TdDataAdapter>(dataAdapter);
        }

        [Fact]
        public void CreateParameter()
        {
            // Arrange
            var teradataDbService = new TeradataDbService(_teradataDbOptions.Object);

            // Act
            var parameter = teradataDbService.CreateParameter();

            // Assert
            Assert.IsType<TdParameter>(parameter);
        }
    }
}
