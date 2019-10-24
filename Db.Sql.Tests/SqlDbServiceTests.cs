namespace Services.Db.Sql.Tests
{
    using System;
    using System.Data.SqlClient;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;
    using static Common.DbServiceType;
    using static Xunit.Assert;

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
            Throws<ArgumentNullException>(TestCode);
        }

        [Fact]
        public void Provider()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object);

            // Assert
            Equal(Sql, sqlDbService.Type);
        }

        [Fact]
        public void Name()
        {
            // Arrange
            const string name = "TestName";
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object)
            {
                Name = name
            };

            // Assert
            Equal(name, sqlDbService.Name);
        }

        [Fact]
        public void CreateCommand()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object);

            // Act
            var command = sqlDbService.CreateCommand();

            // Assert
            IsType<SqlCommand>(command);
        }

        [Fact]
        public void CreateCommandBuilder()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object);

            // Act
            var commandBuilder = sqlDbService.CreateCommandBuilder();

            // Assert
            IsType<SqlCommandBuilder>(commandBuilder);
        }

        [Fact]
        public void CreateConnection()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object);

            // Act
            var connection = sqlDbService.CreateConnection();

            // Assert
            IsType<SqlConnection>(connection);
        }

        [Fact]
        public void CreateConnectionStringBuilder()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object);

            // Act
            var connectionStringBuilder = sqlDbService.CreateConnectionStringBuilder();

            // Assert
            var sqlConnectionStringBuilder = IsType<SqlConnectionStringBuilder>(connectionStringBuilder);
            Equal(_sqlDbOptions.Object.Value.DataSource, sqlConnectionStringBuilder.DataSource);
            Equal(_sqlDbOptions.Object.Value.UserId, sqlConnectionStringBuilder.UserID);
            Equal(_sqlDbOptions.Object.Value.Password, sqlConnectionStringBuilder.Password);
        }

        [Fact]
        public void CreateDataAdapter()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object);

            // Act
            var dataAdapter = sqlDbService.CreateDataAdapter();

            // Assert
            IsType<SqlDataAdapter>(dataAdapter);
        }

        [Fact]
        public void CreateParameter()
        {
            // Arrange
            var sqlDbService = new SqlDbService(_sqlDbOptions.Object);

            // Act
            var parameter = sqlDbService.CreateParameter();

            // Assert
            IsType<SqlParameter>(parameter);
        }
    }
}
