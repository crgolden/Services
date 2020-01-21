//namespace Services.Data.EntityFramework.Tests
//{
//    using System;
//    using System.Linq;
//    using System.Threading.Tasks;
//    using Common;
//    using Microsoft.EntityFrameworkCore;
//    using Microsoft.Extensions.Logging;
//    using Moq;
//    using Xunit;
//    using static System.Guid;
//    using static Common.DataServiceType;
//    using static Common.EventId;
//    using static Microsoft.EntityFrameworkCore.EntityState;
//    using static Microsoft.Extensions.Logging.LogLevel;
//    using static Moq.Mock;
//    using static Moq.Times;

//    public class EntityFrameworkDataCommandServiceTests
//    {
//        private const string DbSetError = "Cannot create a DbSet for 'object' because this type is not included in the model for the context.";
//        private const string EntityTypeError = "The entity type 'object' was not found. Ensure that the entity type has been added to the model.";

//        private static string? DatabaseNamePrefix => typeof(EntityFrameworkDataCommandServiceTests).FullName;

//        [Fact]
//        public void ThrowsForNullDbContext()
//        {
//            // Arrange
//            var logger = Of<ILogger<EntityFrameworkDataQueryService>>();
//            object TestCode() => new EntityFrameworkDataQueryService(default, logger);

//            // Act / Assert
//            var exception = Assert.Throws<ArgumentNullException>(TestCode);
//            Assert.Equal("context", exception.ParamName);
//        }

//        [Fact]
//        public void ThrowsForNullLogger()
//        {
//            // Arrange
//            var context = Of<DbContext>();
//            object TestCode() => new EntityFrameworkDataQueryService(context, default);

//            // Act / Assert
//            var exception = Assert.Throws<ArgumentNullException>(TestCode);
//            Assert.Equal("logger", exception.ParamName);
//        }

//        [Fact]
//        public void Type()
//        {
//            // Arrange
//            var context = Of<DbContext>();
//            var logger = Of<ILogger<EntityFrameworkDataQueryService>>();
//            var service = new EntityFrameworkDataQueryService(context, logger);

//            // Assert
//            Assert.Equal(EntityFramework, service.Type);
//        }

//        [Fact]
//        public void Name()
//        {
//            // Arrange
//            const string name = "TestName";
//            var context = Of<DbContext>();
//            var logger = Of<ILogger<EntityFrameworkDataQueryService>>();
//            var service = new EntityFrameworkDataQueryService(context, logger)
//            {
//                Name = name
//            };

//            // Assert
//            Assert.Equal(name, service.Name);
//        }

//        [Fact]
//        public async Task CreateAsyncThrowsForNullRecord()
//        {
//            // Arrange
//            var context = Of<DbContext>();
//            var logger = Of<ILogger<EntityFrameworkDataQueryService>>();
//            var service = new EntityFrameworkDataQueryService(context, logger);
//            Task TestCode() => service.CreateAsync<object>(default);

//            // Act / Assert
//            var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
//            Assert.Equal("record", exception.ParamName);
//        }

//        [Fact]
//        public async Task CreateAsync()
//        {
//            // Arrange
//            var entity = new TestEntity();
//            var databaseName = $"{DatabaseNamePrefix}.{nameof(CreateAsync)}";
//            var options = new DbContextOptionsBuilder<TestContext>()
//                .UseInMemoryDatabase(databaseName)
//                .Options;
//            var logger = new Mock<ILogger<EntityFrameworkDataQueryService>>();
//            TestEntity response;

//            // Act
//            await using (var context = new TestContext(options))
//            {
//                var service = new EntityFrameworkDataQueryService(context, logger.Object);
//                response = await service.CreateAsync(entity).ConfigureAwait(false);
//            }

//            // Assert
//            Assert.NotEqual(Empty, response.Key);
//            await using (var context = new TestContext(options))
//            {
//                entity = await context.FindAsync<TestEntity>(response.Key).ConfigureAwait(false);
//            }

//            Assert.NotNull(entity);
//            logger.As<ILogger>().Verify(DataCreateStart.IsLoggedWith(Information), Once);
//            logger.As<ILogger>().Verify(DataCreateEnd.IsLoggedWith(Information), Once);
//        }

//        [Fact]
//        public async Task CreateAsyncInvalid()
//        {
//            // Arrange
//            var databaseName = $"{DatabaseNamePrefix}.{nameof(CreateAsyncInvalid)}";
//            var options = new DbContextOptionsBuilder<TestContext>()
//                .UseInMemoryDatabase(databaseName)
//                .Options;
//            var logger = new Mock<ILogger<EntityFrameworkDataQueryService>>();
//            InvalidOperationException exception;

//            // Act
//            await using (var context = new TestContext(options))
//            {
//                var service = new EntityFrameworkDataQueryService(context, logger.Object);
//                Task TestCode() => service.CreateAsync(new object());
//                exception = await Assert.ThrowsAsync<InvalidOperationException>(TestCode).ConfigureAwait(false);
//            }

//            // Assert
//            Assert.Equal(EntityTypeError, exception.Message);
//            logger.As<ILogger>().Verify(DataCreateStart.IsLoggedWith(Information), Once);
//            logger.As<ILogger>().Verify(DataCreateError.IsLoggedWith(Error, exception), Once);
//        }

//        [Fact]
//        public async Task ReadAsyncThrowsForNullExpression()
//        {
//            // Arrange
//            var context = Of<DbContext>();
//            var logger = Of<ILogger<EntityFrameworkDataQueryService>>();
//            var service = new EntityFrameworkDataQueryService(context, logger);
//            Task TestCode() => service.ReadAsync<object>(default);

//            // Act / Assert
//            var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
//            Assert.Equal("expression", exception.ParamName);
//        }

//        [Fact]
//        public async Task ReadAsync()
//        {
//            // Arrange
//            Guid key;
//            var databaseName = $"{DatabaseNamePrefix}.{nameof(ReadAsync)}";
//            var options = new DbContextOptionsBuilder<TestContext>()
//                .UseInMemoryDatabase(databaseName)
//                .Options;
//            var logger = new Mock<ILogger<EntityFrameworkDataQueryService>>();
//            await using (var context = new TestContext(options))
//            {
//                var entity = new TestEntity();
//                context.Entry(entity).State = Added;
//                key = entity.Key;
//                await context.SaveChangesAsync().ConfigureAwait(false);
//            }

//            TestEntity? response;

//            // Act
//            await using (var context = new TestContext(options))
//            {
//                var service = new EntityFrameworkDataQueryService(context, logger.Object);
//                response = await service.ReadAsync<TestEntity>(x => x.Key == key).ConfigureAwait(false);
//            }

//            // Assert
//            Assert.Equal(key, response?.Key);
//            logger.As<ILogger>().Verify(DataReadStart.IsLoggedWith(Information), Once);
//            logger.As<ILogger>().Verify(DataReadEnd.IsLoggedWith(Information), Once);
//        }

//        [Fact]
//        public async Task ReadAsyncInvalid()
//        {
//            // Arrange
//            var databaseName = $"{DatabaseNamePrefix}.{nameof(ReadAsyncInvalid)}";
//            var options = new DbContextOptionsBuilder<TestContext>()
//                .UseInMemoryDatabase(databaseName)
//                .Options;
//            var logger = new Mock<ILogger<EntityFrameworkDataQueryService>>();
//            InvalidOperationException exception;

//            // Act
//            await using (var context = new TestContext(options))
//            {
//                var service = new EntityFrameworkDataQueryService(context, logger.Object);
//                Task TestCode() => service.ReadAsync<object>(x => true);
//                exception = await Assert.ThrowsAsync<InvalidOperationException>(TestCode).ConfigureAwait(false);
//            }

//            // Assert
//            Assert.Equal(DbSetError, exception.Message);
//            logger.As<ILogger>().Verify(DataReadStart.IsLoggedWith(Information), Once);
//            logger.As<ILogger>().Verify(DataReadError.IsLoggedWith(Error, exception), Once);
//        }

//        [Fact]
//        public async Task UpdateAsyncThrowsForNullRecord()
//        {
//            // Arrange
//            var context = Of<DbContext>();
//            var logger = Of<ILogger<EntityFrameworkDataQueryService>>();
//            var service = new EntityFrameworkDataQueryService(context, logger);
//            Task TestCode() => service.UpdateAsync<object>(default, default);

//            // Act / Assert
//            var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
//            Assert.Equal("record", exception.ParamName);
//        }

//        [Fact]
//        public async Task UpdateAsync()
//        {
//            // Arrange
//            const string name1 = "Name1", name2 = "Name2";
//            var entity = new TestEntity
//            {
//                Name = name1
//            };
//            var databaseName = $"{DatabaseNamePrefix}.{nameof(UpdateAsync)}";
//            var options = new DbContextOptionsBuilder<TestContext>()
//                .UseInMemoryDatabase(databaseName)
//                .Options;
//            var logger = new Mock<ILogger<EntityFrameworkDataQueryService>>();
//            await using (var context = new TestContext(options))
//            {
//                context.Entry(entity).State = Added;
//                await context.SaveChangesAsync().ConfigureAwait(false);
//            }

//            // Act
//            entity.Name = name2;
//            await using (var context = new TestContext(options))
//            {
//                var service = new EntityFrameworkDataQueryService(context, logger.Object);
//                await service.UpdateAsync(default, entity).ConfigureAwait(false);
//            }

//            // Assert
//            await using (var context = new TestContext(options))
//            {
//                entity = await context.FindAsync<TestEntity>(entity.Key).ConfigureAwait(false);
//            }

//            Assert.Equal(name2, entity?.Name);
//            logger.As<ILogger>().Verify(DataUpdateStart.IsLoggedWith(Information), Once);
//            logger.As<ILogger>().Verify(DataUpdateEnd.IsLoggedWith(Information), Once);
//        }

//        [Fact]
//        public async Task UpdateAsyncInvalid()
//        {
//            // Arrange
//            var databaseName = $"{DatabaseNamePrefix}.{nameof(UpdateAsyncInvalid)}";
//            var options = new DbContextOptionsBuilder<TestContext>()
//                .UseInMemoryDatabase(databaseName)
//                .Options;
//            var logger = new Mock<ILogger<EntityFrameworkDataQueryService>>();
//            InvalidOperationException exception;

//            // Act
//            await using (var context = new TestContext(options))
//            {
//                var service = new EntityFrameworkDataQueryService(context, logger.Object);
//                Task TestCode() => service.UpdateAsync(default, new object());
//                exception = await Assert.ThrowsAsync<InvalidOperationException>(TestCode).ConfigureAwait(false);
//            }

//            // Assert
//            Assert.Equal(EntityTypeError, exception.Message);
//            logger.As<ILogger>().Verify(DataUpdateStart.IsLoggedWith(Information), Once);
//            logger.As<ILogger>().Verify(DataUpdateError.IsLoggedWith(Error, exception), Once);
//        }

//        [Fact]
//        public async Task DeleteAsyncThrowsForNullExpression()
//        {
//            // Arrange
//            var context = Of<DbContext>();
//            var logger = Of<ILogger<EntityFrameworkDataQueryService>>();
//            var service = new EntityFrameworkDataQueryService(context, logger);
//            Task TestCode() => service.DeleteAsync<object>(default);

//            // Act / Assert
//            var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
//            Assert.Equal("expression", exception.ParamName);
//        }

//        [Fact]
//        public async Task DeleteAsync()
//        {
//            // Arrange
//            Guid key;
//            var databaseName = $"{DatabaseNamePrefix}.{nameof(DeleteAsync)}";
//            var options = new DbContextOptionsBuilder<TestContext>()
//                .UseInMemoryDatabase(databaseName)
//                .Options;
//            var logger = new Mock<ILogger<EntityFrameworkDataQueryService>>();
//            await using (var context = new TestContext(options))
//            {
//                var entity = new TestEntity();
//                context.Entry(entity).State = Added;
//                key = entity.Key;
//                await context.SaveChangesAsync().ConfigureAwait(false);
//            }

//            TestEntity? response;

//            // Act
//            await using (var context = new TestContext(options))
//            {
//                var service = new EntityFrameworkDataQueryService(context, logger.Object);
//                await service.DeleteAsync<TestEntity>(x => x.Key == key).ConfigureAwait(false);
//            }

//            // Assert
//            await using (var context = new TestContext(options))
//            {
//                response = await context.FindAsync<TestEntity>(key).ConfigureAwait(false);
//            }

//            Assert.Null(response);
//            logger.As<ILogger>().Verify(DataDeleteStart.IsLoggedWith(Information), Once);
//            logger.As<ILogger>().Verify(DataDeleteEnd.IsLoggedWith(Information), Once);
//        }

//        [Fact]
//        public async Task DeleteAsyncInvalid()
//        {
//            // Arrange
//            var databaseName = $"{DatabaseNamePrefix}.{nameof(DeleteAsyncInvalid)}";
//            var options = new DbContextOptionsBuilder<TestContext>()
//                .UseInMemoryDatabase(databaseName)
//                .Options;
//            var logger = new Mock<ILogger<EntityFrameworkDataQueryService>>();
//            InvalidOperationException exception;

//            // Act
//            await using (var context = new TestContext(options))
//            {
//                var service = new EntityFrameworkDataQueryService(context, logger.Object);
//                Task TestCode() => service.DeleteAsync<object>(x => true);
//                exception = await Assert.ThrowsAsync<InvalidOperationException>(TestCode).ConfigureAwait(false);
//            }

//            // Assert
//            Assert.Equal(DbSetError, exception.Message);
//            logger.As<ILogger>().Verify(DataDeleteStart.IsLoggedWith(Information), Once);
//            logger.As<ILogger>().Verify(DataDeleteError.IsLoggedWith(Error, exception), Once);
//        }

//        [Fact]
//        public void List()
//        {
//            // Arrange
//            var databaseName = $"{DatabaseNamePrefix}.{nameof(List)}";
//            var options = new DbContextOptionsBuilder<TestContext>()
//                .UseInMemoryDatabase(databaseName)
//                .Options;
//            var logger = new Mock<ILogger<EntityFrameworkDataQueryService>>();

//            IQueryable<object> response;

//            // Act
//            using (var context = new TestContext(options))
//            {
//                var service = new EntityFrameworkDataQueryService(context, logger.Object);
//                response = service.Query<object>();
//            }

//            // Assert
//            Assert.IsAssignableFrom<DbSet<object>>(response);
//            logger.As<ILogger>().Verify(DataListStart.IsLoggedWith(Information), Once);
//            logger.As<ILogger>().Verify(DataListEnd.IsLoggedWith(Information), Once);
//        }

//        [Fact]
//        public void ListInvalid()
//        {
//            // Arrange
//            var context = new Mock<DbContext>();
//            context.Setup(x => x.Set<object>()).Throws<InvalidOperationException>();
//            var logger = new Mock<ILogger<EntityFrameworkDataQueryService>>();
//            var service = new EntityFrameworkDataQueryService(context.Object, logger.Object);
//            object TestCode() => service.Query<object>();

//            // Act / Assert
//            var exception = Assert.Throws<InvalidOperationException>(TestCode);
//            logger.As<ILogger>().Verify(DataListStart.IsLoggedWith(Information), Once);
//            logger.As<ILogger>().Verify(DataListError.IsLoggedWith(Error, exception), Once);
//        }

//        private class TestContext : DbContext
//        {
//            public TestContext(DbContextOptions<TestContext> options)
//                : base(options)
//            {
//            }

//            protected override void OnModelCreating(ModelBuilder modelBuilder)
//            {
//                base.OnModelCreating(modelBuilder);
//                modelBuilder.Entity<TestEntity>(e =>
//                {
//                    e.HasKey(x => x.Key);
//                    e.Property(x => x.Name);
//                });
//            }
//        }

//        private class TestEntity
//        {
//            public Guid Key { get; private set; } = NewGuid();

//            public string? Name { get; set; }
//        }
//    }
//}
