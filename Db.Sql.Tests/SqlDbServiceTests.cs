namespace Services.Db.Sql.Tests
{
    using System;
    using System.Data.SqlClient;
    using Xunit;

    public class SqlDbServiceTests
    {
        private readonly SqlConnectionStringBuilder _sqlConnectionStringBuilder;

        public SqlDbServiceTests()
        {
            _sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
        }

        [Fact]
        public void ThrowsForInvalidOptions()
        {
            // Arrange
            SqlDbService TestCode() => new SqlDbService(default);

            // Assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        [Fact]
        public void Name()
        {
            // Arrange
            const string name = "TestName";
            var sqlDbService = new SqlDbService(_sqlConnectionStringBuilder)
            {
                Name = name
            };

            // Assert
            Assert.Equal(name, sqlDbService.Name);
        }

        [Fact]
        public void CreateCommand()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlConnectionStringBuilder);

            // Act
            var command = sqlDbService.CreateCommand();

            // Assert
            Assert.IsType<SqlCommand>(command);
        }

        [Fact]
        public void CreateCommandBuilder()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlConnectionStringBuilder);

            // Act
            var commandBuilder = sqlDbService.CreateCommandBuilder();

            // Assert
            Assert.IsType<SqlCommandBuilder>(commandBuilder);
        }

        [Fact]
        public void CreateConnection()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlConnectionStringBuilder);

            // Act
            var connection = sqlDbService.CreateConnection();

            // Assert
            Assert.IsType<SqlConnection>(connection);
        }

        [Fact]
        public void CreateConnectionStringBuilder()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlConnectionStringBuilder);

            // Act
            var connectionStringBuilder = sqlDbService.CreateConnectionStringBuilder();

            // Assert
            Assert.IsType<SqlConnectionStringBuilder>(connectionStringBuilder);
        }

        [Fact]
        public void CreateDataAdapter()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlConnectionStringBuilder);

            // Act
            var dataAdapter = sqlDbService.CreateDataAdapter();

            // Assert
            Assert.IsType<SqlDataAdapter>(dataAdapter);
        }

        [Fact]
        public void CreateParameter()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlConnectionStringBuilder);

            // Act
            var parameter = sqlDbService.CreateParameter();

            // Assert
            Assert.IsType<SqlParameter>(parameter);
        }
    }
}
