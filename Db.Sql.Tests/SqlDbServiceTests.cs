namespace Services.Db.Sql.Tests
{
    using System;
    using System.Data.SqlClient;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class SqlDbServiceTests
    {
        private readonly Mock<IOptions<SqlDbOptions>> _sqlDbOptions;

        public SqlDbServiceTests()
        {
            _sqlDbOptions = new Mock<IOptions<SqlDbOptions>>();
            _sqlDbOptions.Setup(x => x.Value).Returns(new SqlDbOptions
            {
                DataSource = nameof(SqlDbOptions.DataSource),
                UserId = nameof(SqlDbOptions.UserId),
                Password = nameof(SqlDbOptions.Password)
            });
        }

        [Fact]
        public void ThrowsForInvalidOptions()
        {
            // Arrange
            static object TestCode() => new SqlDbService(default);

            // Assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        [Fact]
        public void Provider()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object);

            // Assert
            Assert.Equal("Sql", sqlDbService.Provider);
        }

        [Fact]
        public void CreateCommand()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object);

            // Act
            var command = sqlDbService.CreateCommand();

            // Assert
            Assert.IsType<SqlCommand>(command);
        }

        [Fact]
        public void CreateCommandBuilder()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object);

            // Act
            var commandBuilder = sqlDbService.CreateCommandBuilder();

            // Assert
            Assert.IsType<SqlCommandBuilder>(commandBuilder);
        }

        [Fact]
        public void CreateConnection()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object);

            // Act
            var connection = sqlDbService.CreateConnection();

            // Assert
            Assert.IsType<SqlConnection>(connection);
        }

        [Fact]
        public void CreateConnectionStringBuilder()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object);

            // Act
            var connectionStringBuilder = sqlDbService.CreateConnectionStringBuilder();

            // Assert
            var sqlConnectionStringBuilder = Assert.IsType<SqlConnectionStringBuilder>(connectionStringBuilder);
            Assert.Equal(_sqlDbOptions.Object.Value.DataSource, sqlConnectionStringBuilder.DataSource);
            Assert.Equal(_sqlDbOptions.Object.Value.UserId, sqlConnectionStringBuilder.UserID);
            Assert.Equal(_sqlDbOptions.Object.Value.Password, sqlConnectionStringBuilder.Password);
        }

        [Fact]
        public void CreateDataAdapter()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object);

            // Act
            var dataAdapter = sqlDbService.CreateDataAdapter();

            // Assert
            Assert.IsType<SqlDataAdapter>(dataAdapter);
        }

        [Fact]
        public void CreateParameter()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object);

            // Act
            var parameter = sqlDbService.CreateParameter();

            // Assert
            Assert.IsType<SqlParameter>(parameter);
        }
    }
}
