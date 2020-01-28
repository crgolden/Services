namespace Services.Data.Mongo.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;
    using Moq;
    using Xunit;
    using static System.Threading.Tasks.Task;
    using static Common.EventId;
    using static Microsoft.Extensions.Logging.LogLevel;
    using static Moq.Mock;
    using static Moq.Times;

    public class MongoDataServiceTests
    {
        [Fact]
        public void ThrowsForNullName()
        {
            // Arrange
            var client = Of<IMongoClient>();
            var options = new MongoDataOptions();
            object TestCode() => new MongoDataService(client, options, default);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("name", exception.ParamName);
        }

        [Fact]
        public void ThrowsForNullMongoDatabase()
        {
            // Arrange
            var logger = Of<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions());
            object TestCode() => new MongoDataService(default, logger, options.Object);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("database", exception.ParamName);
        }

        [Fact]
        public void ThrowsForNullLogger()
        {
            // Arrange
            var database = Of<IMongoDatabase>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions());
            object TestCode() => new MongoDataService(database, default, options.Object);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("logger", exception.ParamName);
        }

        [Fact]
        public void ThrowsForNullOptions()
        {
            // Arrange
            var database = Of<IMongoDatabase>();
            var logger = Of<ILogger<MongoDataService>>();
            object TestCode() => new MongoDataService(database, logger, default);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("mongoDataOptions", exception.ParamName);
        }

        [Fact]
        public void Type()
        {
            // Arrange
            var database = Of<IMongoDatabase>();
            var logger = Of<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions());
            var service = new MongoDataService(database, logger, options.Object);

            // Assert
            Assert.Equal(Mongo, service.Type);
        }

        [Fact]
        public void Name()
        {
            // Arrange
            const string name = "TestName";
            var database = Of<IMongoDatabase>();
            var logger = Of<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions());
            var service = new MongoDataService(database, logger, options.Object)
            {
                Name = name
            };

            // Assert
            Assert.Equal(name, service.Name);
        }

        [Fact]
        public async Task CreateAsyncThrowsForMissingCollectionName()
        {
            // Arrange
            var database = Of<IMongoDatabase>();
            var logger = Of<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions());
            var service = new MongoDataService(database, logger, options.Object);
            Task TestCode() => service.CreateAsync<object>(default);

            // Act / Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(TestCode).ConfigureAwait(false);
            Assert.Equal($"Collection name not found for type '{typeof(object).Name}'", exception.Message);
        }

        [Fact]
        public async Task CreateAsyncThrowsForNullRecord()
        {
            // Arrange
            var database = Of<IMongoDatabase>();
            var logger = Of<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database, logger, options.Object);
            Task TestCode() => service.CreateAsync<object>(default);

            // Act / Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
            Assert.Equal("record", exception.ParamName);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var collection = new Mock<IMongoCollection<object>>();
            var database = new Mock<IMongoDatabase>();
            database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Returns(collection.Object);
            var logger = new Mock<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database.Object, logger.Object, options.Object);

            // Act
            var response = await service.CreateAsync(new object()).ConfigureAwait(false);

            // Assert
            collection.Verify(x => x.InsertOneAsync(response, It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Once);
            logger.As<ILogger>().Verify(DataCreateStart.IsLoggedWith(Information), Once);
            logger.As<ILogger>().Verify(DataCreateEnd.IsLoggedWith(Information), Once);
        }

        [Fact]
        public async Task CreateAsyncInvalid()
        {
            // Arrange
            var exception = new Exception("TestException");
            var database = new Mock<IMongoDatabase>();
            database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Throws(exception);
            var logger = new Mock<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database.Object, logger.Object, options.Object);
            Task TestCode() => service.CreateAsync(new object());

            // Act
            var response = await Assert.ThrowsAsync<Exception>(TestCode).ConfigureAwait(false);

            // Assert
            Assert.Equal(exception.Message, response.Message);
            logger.As<ILogger>().Verify(DataCreateStart.IsLoggedWith(Information), Once);
            logger.As<ILogger>().Verify(DataCreateError.IsLoggedWith(Error, exception), Once);
        }

        [Fact]
        public async Task ReadAsyncThrowsForNullExpression()
        {
            // Arrange
            var database = Of<IMongoDatabase>();
            var logger = Of<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database, logger, options.Object);
            Task TestCode() => service.ReadAsync<object>(default);

            // Act / Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
            Assert.Equal("expression", exception.ParamName);
        }

        [Fact]
        public async Task ReadAsync()
        {
            // Arrange
            var cursor = new Mock<IAsyncCursor<object>>();
            cursor.Setup(x => x.Current).Returns(new[] { new object() });
            cursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>())).Returns(FromResult(true)).Returns(FromResult(false));
            var collection = new Mock<IMongoCollection<object>>();
            collection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<object>>(), It.IsAny<FindOptions<object, object>>(), It.IsAny<CancellationToken>())).ReturnsAsync(cursor.Object);
            var database = new Mock<IMongoDatabase>();
            database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Returns(collection.Object);
            var logger = new Mock<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database.Object, logger.Object, options.Object);

            // Act
            var response = await service.ReadAsync<object>(x => true).ConfigureAwait(false);

            // Assert
            Assert.NotNull(response);
            logger.As<ILogger>().Verify(DataReadStart.IsLoggedWith(Information), Once);
            logger.As<ILogger>().Verify(DataReadEnd.IsLoggedWith(Information), Once);
        }

        [Fact]
        public async Task ReadAsyncInvalid()
        {
            // Arrange
            var exception = new Exception("TestException");
            var database = new Mock<IMongoDatabase>();
            database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Throws(exception);
            var logger = new Mock<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database.Object, logger.Object, options.Object);
            Task TestCode() => service.ReadAsync<object>(x => true);

            // Act
            var response = await Assert.ThrowsAsync<Exception>(TestCode).ConfigureAwait(false);

            // Assert
            Assert.Equal(exception.Message, response.Message);
            logger.As<ILogger>().Verify(DataReadStart.IsLoggedWith(Information), Once);
            logger.As<ILogger>().Verify(DataReadError.IsLoggedWith(Error, exception), Once);
        }

        [Fact]
        public async Task UpdateAsyncThrowsForNullExpression()
        {
            // Arrange
            var database = Of<IMongoDatabase>();
            var logger = Of<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database, logger, options.Object);
            Task TestCode() => service.UpdateAsync(default, new object());

            // Act / Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
            Assert.Equal("expression", exception.ParamName);
        }

        [Fact]
        public async Task UpdateAsyncThrowsForNullRecord()
        {
            // Arrange
            var database = Of<IMongoDatabase>();
            var logger = Of<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database, logger, options.Object);
            Task TestCode() => service.UpdateAsync<object>(x => true, default);

            // Act / Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
            Assert.Equal("record", exception.ParamName);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var collection = new Mock<IMongoCollection<object>>();
            var database = new Mock<IMongoDatabase>();
            database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Returns(collection.Object);
            var logger = new Mock<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database.Object, logger.Object, options.Object);

            // Act
            await service.UpdateAsync(x => true, new object()).ConfigureAwait(false);

            // Assert
            collection.Verify(x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<object>>(), It.IsAny<object>(), It.IsAny<UpdateOptions>(), It.IsAny<CancellationToken>()), Once);
            logger.As<ILogger>().Verify(DataUpdateStart.IsLoggedWith(Information), Once);
            logger.As<ILogger>().Verify(DataUpdateEnd.IsLoggedWith(Information), Once);
        }

        [Fact]
        public async Task UpdateAsyncInvalid()
        {
            // Arrange
            var exception = new Exception("TestException");
            var database = new Mock<IMongoDatabase>();
            database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Throws(exception);
            var logger = new Mock<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database.Object, logger.Object, options.Object);
            Task TestCode() => service.UpdateAsync(x => true, new object());

            // Act
            var response = await Assert.ThrowsAsync<Exception>(TestCode).ConfigureAwait(false);

            // Assert
            Assert.Equal(exception.Message, response.Message);
            logger.As<ILogger>().Verify(DataUpdateStart.IsLoggedWith(Information), Once);
            logger.As<ILogger>().Verify(DataUpdateError.IsLoggedWith(Error, exception), Once);
        }

        [Fact]
        public async Task DeleteAsyncThrowsForNullExpression()
        {
            // Arrange
            var database = Of<IMongoDatabase>();
            var logger = Of<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database, logger, options.Object);
            Task TestCode() => service.DeleteAsync<object>(default);

            // Act / Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
            Assert.Equal("expression", exception.ParamName);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var collection = new Mock<IMongoCollection<object>>();
            var database = new Mock<IMongoDatabase>();
            database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Returns(collection.Object);
            var logger = new Mock<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database.Object, logger.Object, options.Object);

            // Act
            await service.DeleteAsync<object>(x => true).ConfigureAwait(false);

            // Assert
            collection.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<object>>(), It.IsAny<DeleteOptions>(), It.IsAny<CancellationToken>()), Once);
            logger.As<ILogger>().Verify(DataDeleteStart.IsLoggedWith(Information), Once);
            logger.As<ILogger>().Verify(DataDeleteEnd.IsLoggedWith(Information), Once);
        }

        [Fact]
        public async Task DeleteAsyncInvalid()
        {
            // Arrange
            var exception = new Exception("TestException");
            var database = new Mock<IMongoDatabase>();
            database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Throws(exception);
            var logger = new Mock<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database.Object, logger.Object, options.Object);
            Task TestCode() => service.DeleteAsync<object>(x => true);

            // Act
            var response = await Assert.ThrowsAsync<Exception>(TestCode).ConfigureAwait(false);

            // Assert
            Assert.Equal(exception.Message, response.Message);
            logger.As<ILogger>().Verify(DataDeleteStart.IsLoggedWith(Information), Once);
            logger.As<ILogger>().Verify(DataDeleteError.IsLoggedWith(Error, exception), Once);
        }

        [Fact]
        public void List()
        {
            // Arrange
            var collection = new Mock<IMongoCollection<object>>();
            var database = new Mock<IMongoDatabase>();
            database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Returns(collection.Object);
            var logger = new Mock<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database.Object, logger.Object, options.Object);

            // Act
            service.Query<object>();

            // Assert
            logger.As<ILogger>().Verify(DataListStart.IsLoggedWith(Information), Once);
            logger.As<ILogger>().Verify(DataListEnd.IsLoggedWith(Information), Once);
        }

        [Fact]
        public void ListInvalid()
        {
            // Arrange
            var exception = new Exception("TestException");
            var database = new Mock<IMongoDatabase>();
            database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Throws(exception);
            var logger = new Mock<ILogger<MongoDataService>>();
            var options = new MongoDataOptions();
            options.Setup(x => x.Value).Returns(new MongoDataOptions
            {
                CollectionNames = new Dictionary<string, string>
                {
                    { typeof(object).Name, "Objects" }
                }
            });
            var service = new MongoDataService(database.Object, logger.Object, options.Object);
            object TestCode() => service.Query<object>();

            // Act
            var response = Assert.Throws<Exception>(TestCode);

            // Assert
            Assert.Equal(exception.Message, response.Message);
            logger.As<ILogger>().Verify(DataListStart.IsLoggedWith(Information), Once);
            logger.As<ILogger>().Verify(DataListError.IsLoggedWith(Error, exception), Once);
        }
    }
}
