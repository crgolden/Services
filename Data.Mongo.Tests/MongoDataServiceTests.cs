namespace Services.Data.Mongo.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using MongoDB.Driver;
    using Moq;
    using Xunit;
    using static System.Guid;
    using static System.Threading.Tasks.Task;
    using static Constants.ExceptionMessages;
    using static Moq.Mock;
    using static Moq.Times;

    public class MongoDataServiceTests
    {
        private readonly Mock<IMongoClient> _mongoClient;
        private readonly Mock<IMongoDatabase> _mongoDatabase;
        private readonly Mock<IMongoCollection<object>> _mongoCollection;
        private readonly MongoDataOptions _options;
        private readonly string _name;

        public MongoDataServiceTests()
        {
            _mongoClient = new Mock<IMongoClient>();
            _mongoDatabase = new Mock<IMongoDatabase>();
            _mongoCollection = new Mock<IMongoCollection<object>>();
            _mongoDatabase.Setup(x => x.GetCollection<object>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>())).Returns(_mongoCollection.Object);
            _mongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>())).Returns(_mongoDatabase.Object);
            _options = new MongoDataOptions();
            _options.CollectionNames.TryAdd(typeof(object).Name, "Objects");
            _name = NewGuid().ToString();
        }

        [Fact]
        public void ThrowsForNullMongoClient()
        {
            // Arrange
            MongoDataService TestCode() => new MongoDataService(default, _options, _name);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("mongoClient", exception.ParamName);
        }

        [Fact]
        public void ThrowsForNullOptions()
        {
            // Arrange
            object TestCode() => new MongoDataService(_mongoClient.Object, default, _name);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("mongoDataOptions", exception.ParamName);
        }

        [Fact]
        public void ThrowsForNullName()
        {
            // Arrange
            MongoDataService TestCode() => new MongoDataService(_mongoClient.Object, _options, default);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("name", exception.ParamName);
        }

        [Fact]
        public void Name()
        {
            // Arrange / Act
            using (var service = new MongoDataService(_mongoClient.Object, _options, _name))
            {
                // Assert
                Assert.Equal(_name, service.Name);
            }
        }

        [Fact]
        public async Task CreateAsyncThrowsForNullRecord()
        {
            // Arrange
            Task<object> TestCode()
            {
                using (var service = new MongoDataService(_mongoClient.Object, _options, _name))
                {
                    // Act
                    return service.CreateAsync<object>(default);
                }
            }

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(true);
            Assert.Equal("record", exception.ParamName);
        }

        [Fact]
        public async Task CreateAsyncThrowsForMissingCollectionName()
        {
            // Arrange
            Task<object> TestCode()
            {
                using (var service = new MongoDataService(_mongoClient.Object, new MongoDataOptions(), _name))
                {
                    return service.CreateAsync(new object());
                }
            }

            // Act / Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(TestCode).ConfigureAwait(false);
            Assert.Equal(CollectionNameNotFound(typeof(object).Name), exception.Message);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            object response;

            // Act
            using (var service = new MongoDataService(_mongoClient.Object, _options, _name))
            {
                response = await service.CreateAsync(new object()).ConfigureAwait(false);
            }

            // Assert
            _mongoCollection.Verify(x => x.InsertOneAsync(response, It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Once);
        }

        //[Fact]
        //public async Task ReadAsyncThrowsForNullExpression()
        //{
        //    // Arrange
        //    var database = Of<IMongoDatabase>();
        //    var logger = Of<ILogger<MongoDataService>>();
        //    var options = new MongoDataOptions();
        //    options.Setup(x => x.Value).Returns(new MongoDataOptions
        //    {
        //        CollectionNames = new Dictionary<string, string>
        //        {
        //            { typeof(object).Name, "Objects" }
        //        }
        //    });
        //    var service = new MongoDataService(database, logger, options.Object);
        //    Task TestCode() => service.ReadAsync<object>(default);

        //    // Act / Assert
        //    var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
        //    Assert.Equal("expression", exception.ParamName);
        //}

        //[Fact]
        //public async Task ReadAsync()
        //{
        //    // Arrange
        //    var cursor = new Mock<IAsyncCursor<object>>();
        //    cursor.Setup(x => x.Current).Returns(new[] { new object() });
        //    cursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>())).Returns(FromResult(true)).Returns(FromResult(false));
        //    var collection = new Mock<IMongoCollection<object>>();
        //    collection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<object>>(), It.IsAny<FindOptions<object, object>>(), It.IsAny<CancellationToken>())).ReturnsAsync(cursor.Object);
        //    var database = new Mock<IMongoDatabase>();
        //    database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Returns(collection.Object);
        //    var logger = new Mock<ILogger<MongoDataService>>();
        //    var options = new MongoDataOptions();
        //    options.Setup(x => x.Value).Returns(new MongoDataOptions
        //    {
        //        CollectionNames = new Dictionary<string, string>
        //        {
        //            { typeof(object).Name, "Objects" }
        //        }
        //    });
        //    var service = new MongoDataService(database.Object, logger.Object, options.Object);

        //    // Act
        //    var response = await service.ReadAsync<object>(x => true).ConfigureAwait(false);

        //    // Assert
        //    Assert.NotNull(response);
        //    logger.As<ILogger>().Verify(DataReadStart.IsLoggedWith(Information), Once);
        //    logger.As<ILogger>().Verify(DataReadEnd.IsLoggedWith(Information), Once);
        //}

        //[Fact]
        //public async Task ReadAsyncInvalid()
        //{
        //    // Arrange
        //    var exception = new Exception("TestException");
        //    var database = new Mock<IMongoDatabase>();
        //    database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Throws(exception);
        //    var logger = new Mock<ILogger<MongoDataService>>();
        //    var options = new MongoDataOptions();
        //    options.Setup(x => x.Value).Returns(new MongoDataOptions
        //    {
        //        CollectionNames = new Dictionary<string, string>
        //        {
        //            { typeof(object).Name, "Objects" }
        //        }
        //    });
        //    var service = new MongoDataService(database.Object, logger.Object, options.Object);
        //    Task TestCode() => service.ReadAsync<object>(x => true);

        //    // Act
        //    var response = await Assert.ThrowsAsync<Exception>(TestCode).ConfigureAwait(false);

        //    // Assert
        //    Assert.Equal(exception.Message, response.Message);
        //    logger.As<ILogger>().Verify(DataReadStart.IsLoggedWith(Information), Once);
        //    logger.As<ILogger>().Verify(DataReadError.IsLoggedWith(Error, exception), Once);
        //}

        //[Fact]
        //public async Task UpdateAsyncThrowsForNullExpression()
        //{
        //    // Arrange
        //    var database = Of<IMongoDatabase>();
        //    var logger = Of<ILogger<MongoDataService>>();
        //    var options = new MongoDataOptions();
        //    options.Setup(x => x.Value).Returns(new MongoDataOptions
        //    {
        //        CollectionNames = new Dictionary<string, string>
        //        {
        //            { typeof(object).Name, "Objects" }
        //        }
        //    });
        //    var service = new MongoDataService(database, logger, options.Object);
        //    Task TestCode() => service.UpdateAsync(default, new object());

        //    // Act / Assert
        //    var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
        //    Assert.Equal("expression", exception.ParamName);
        //}

        //[Fact]
        //public async Task UpdateAsyncThrowsForNullRecord()
        //{
        //    // Arrange
        //    var database = Of<IMongoDatabase>();
        //    var logger = Of<ILogger<MongoDataService>>();
        //    var options = new MongoDataOptions();
        //    options.Setup(x => x.Value).Returns(new MongoDataOptions
        //    {
        //        CollectionNames = new Dictionary<string, string>
        //        {
        //            { typeof(object).Name, "Objects" }
        //        }
        //    });
        //    var service = new MongoDataService(database, logger, options.Object);
        //    Task TestCode() => service.UpdateAsync<object>(x => true, default);

        //    // Act / Assert
        //    var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
        //    Assert.Equal("record", exception.ParamName);
        //}

        //[Fact]
        //public async Task UpdateAsync()
        //{
        //    // Arrange
        //    var collection = new Mock<IMongoCollection<object>>();
        //    var database = new Mock<IMongoDatabase>();
        //    database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Returns(collection.Object);
        //    var logger = new Mock<ILogger<MongoDataService>>();
        //    var options = new MongoDataOptions();
        //    options.Setup(x => x.Value).Returns(new MongoDataOptions
        //    {
        //        CollectionNames = new Dictionary<string, string>
        //        {
        //            { typeof(object).Name, "Objects" }
        //        }
        //    });
        //    var service = new MongoDataService(database.Object, logger.Object, options.Object);

        //    // Act
        //    await service.UpdateAsync(x => true, new object()).ConfigureAwait(false);

        //    // Assert
        //    collection.Verify(x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<object>>(), It.IsAny<object>(), It.IsAny<UpdateOptions>(), It.IsAny<CancellationToken>()), Once);
        //    logger.As<ILogger>().Verify(DataUpdateStart.IsLoggedWith(Information), Once);
        //    logger.As<ILogger>().Verify(DataUpdateEnd.IsLoggedWith(Information), Once);
        //}

        //[Fact]
        //public async Task UpdateAsyncInvalid()
        //{
        //    // Arrange
        //    var exception = new Exception("TestException");
        //    var database = new Mock<IMongoDatabase>();
        //    database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Throws(exception);
        //    var logger = new Mock<ILogger<MongoDataService>>();
        //    var options = new MongoDataOptions();
        //    options.Setup(x => x.Value).Returns(new MongoDataOptions
        //    {
        //        CollectionNames = new Dictionary<string, string>
        //        {
        //            { typeof(object).Name, "Objects" }
        //        }
        //    });
        //    var service = new MongoDataService(database.Object, logger.Object, options.Object);
        //    Task TestCode() => service.UpdateAsync(x => true, new object());

        //    // Act
        //    var response = await Assert.ThrowsAsync<Exception>(TestCode).ConfigureAwait(false);

        //    // Assert
        //    Assert.Equal(exception.Message, response.Message);
        //    logger.As<ILogger>().Verify(DataUpdateStart.IsLoggedWith(Information), Once);
        //    logger.As<ILogger>().Verify(DataUpdateError.IsLoggedWith(Error, exception), Once);
        //}

        //[Fact]
        //public async Task DeleteAsyncThrowsForNullExpression()
        //{
        //    // Arrange
        //    var database = Of<IMongoDatabase>();
        //    var logger = Of<ILogger<MongoDataService>>();
        //    var options = new MongoDataOptions();
        //    options.Setup(x => x.Value).Returns(new MongoDataOptions
        //    {
        //        CollectionNames = new Dictionary<string, string>
        //        {
        //            { typeof(object).Name, "Objects" }
        //        }
        //    });
        //    var service = new MongoDataService(database, logger, options.Object);
        //    Task TestCode() => service.DeleteAsync<object>(default);

        //    // Act / Assert
        //    var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
        //    Assert.Equal("expression", exception.ParamName);
        //}

        //[Fact]
        //public async Task DeleteAsync()
        //{
        //    // Arrange
        //    var collection = new Mock<IMongoCollection<object>>();
        //    var database = new Mock<IMongoDatabase>();
        //    database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Returns(collection.Object);
        //    var logger = new Mock<ILogger<MongoDataService>>();
        //    var options = new MongoDataOptions();
        //    options.Setup(x => x.Value).Returns(new MongoDataOptions
        //    {
        //        CollectionNames = new Dictionary<string, string>
        //        {
        //            { typeof(object).Name, "Objects" }
        //        }
        //    });
        //    var service = new MongoDataService(database.Object, logger.Object, options.Object);

        //    // Act
        //    await service.DeleteAsync<object>(x => true).ConfigureAwait(false);

        //    // Assert
        //    collection.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<object>>(), It.IsAny<DeleteOptions>(), It.IsAny<CancellationToken>()), Once);
        //    logger.As<ILogger>().Verify(DataDeleteStart.IsLoggedWith(Information), Once);
        //    logger.As<ILogger>().Verify(DataDeleteEnd.IsLoggedWith(Information), Once);
        //}

        //[Fact]
        //public async Task DeleteAsyncInvalid()
        //{
        //    // Arrange
        //    var exception = new Exception("TestException");
        //    var database = new Mock<IMongoDatabase>();
        //    database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Throws(exception);
        //    var logger = new Mock<ILogger<MongoDataService>>();
        //    var options = new MongoDataOptions();
        //    options.Setup(x => x.Value).Returns(new MongoDataOptions
        //    {
        //        CollectionNames = new Dictionary<string, string>
        //        {
        //            { typeof(object).Name, "Objects" }
        //        }
        //    });
        //    var service = new MongoDataService(database.Object, logger.Object, options.Object);
        //    Task TestCode() => service.DeleteAsync<object>(x => true);

        //    // Act
        //    var response = await Assert.ThrowsAsync<Exception>(TestCode).ConfigureAwait(false);

        //    // Assert
        //    Assert.Equal(exception.Message, response.Message);
        //    logger.As<ILogger>().Verify(DataDeleteStart.IsLoggedWith(Information), Once);
        //    logger.As<ILogger>().Verify(DataDeleteError.IsLoggedWith(Error, exception), Once);
        //}

        //[Fact]
        //public void List()
        //{
        //    // Arrange
        //    var collection = new Mock<IMongoCollection<object>>();
        //    var database = new Mock<IMongoDatabase>();
        //    database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Returns(collection.Object);
        //    var logger = new Mock<ILogger<MongoDataService>>();
        //    var options = new MongoDataOptions();
        //    options.Setup(x => x.Value).Returns(new MongoDataOptions
        //    {
        //        CollectionNames = new Dictionary<string, string>
        //        {
        //            { typeof(object).Name, "Objects" }
        //        }
        //    });
        //    var service = new MongoDataService(database.Object, logger.Object, options.Object);

        //    // Act
        //    service.Query<object>();

        //    // Assert
        //    logger.As<ILogger>().Verify(DataListStart.IsLoggedWith(Information), Once);
        //    logger.As<ILogger>().Verify(DataListEnd.IsLoggedWith(Information), Once);
        //}

        //[Fact]
        //public void ListInvalid()
        //{
        //    // Arrange
        //    var exception = new Exception("TestException");
        //    var database = new Mock<IMongoDatabase>();
        //    database.Setup(x => x.GetCollection<object>("Objects", It.IsAny<MongoCollectionSettings>())).Throws(exception);
        //    var logger = new Mock<ILogger<MongoDataService>>();
        //    var options = new MongoDataOptions();
        //    options.Setup(x => x.Value).Returns(new MongoDataOptions
        //    {
        //        CollectionNames = new Dictionary<string, string>
        //        {
        //            { typeof(object).Name, "Objects" }
        //        }
        //    });
        //    var service = new MongoDataService(database.Object, logger.Object, options.Object);
        //    object TestCode() => service.Query<object>();

        //    // Act
        //    var response = Assert.Throws<Exception>(TestCode);

        //    // Assert
        //    Assert.Equal(exception.Message, response.Message);
        //    logger.As<ILogger>().Verify(DataListStart.IsLoggedWith(Information), Once);
        //    logger.As<ILogger>().Verify(DataListError.IsLoggedWith(Error, exception), Once);
        //}
    }
}
