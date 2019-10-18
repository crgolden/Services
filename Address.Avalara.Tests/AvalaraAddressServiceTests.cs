namespace Services.Address.Avalara.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Logging;
    using Models;
    using Moq;
    using Xunit;
    using static System.Net.HttpStatusCode;
    using static Common.EventId;
    using static Microsoft.Extensions.Logging.LogLevel;
    using EventId = Microsoft.Extensions.Logging.EventId;

    public class AvalaraAddressServiceTests
    {
        [Fact]
        public void ThrowsForNullHttpClientFactory()
        {
            // Arrange
            var logger = Mock.Of<ILogger<AvalaraAddressService>>();
            object TestCode() => new AvalaraAddressService(default, logger);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("httpClientFactory", exception.ParamName);
        }

        [Fact]
        public void ThrowsForNullLogger()
        {
            // Arrange
            var httpClientFactory = Mock.Of<IHttpClientFactory>();
            object TestCode() => new AvalaraAddressService(httpClientFactory, default);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("logger", exception.ParamName);
        }

        [Fact]
        public async Task ValidateAsyncThrowsForNullAddress()
        {
            // Arrange
            var httpClientFactory = Mock.Of<IHttpClientFactory>();
            var logger = Mock.Of<ILogger<AvalaraAddressService>>();
            var service = new AvalaraAddressService(httpClientFactory, logger);
            async Task TestCode() => await service.ValidateAsync(default).ConfigureAwait(false);

            // Act // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
            Assert.Equal("address", exception.ParamName);
        }

        [Fact]
        public async Task ValidateAsync()
        {
            // Arrange
            var addressResolution = new AddressResolutionModel
            {
                ValidatedAddresses = new List<ValidatedAddressInfo>
                {
                    new ValidatedAddressInfo
                    {
                        Country = "US"
                    }
                }
            };
            var messageHandler = new MockMessageHandler(req =>
            {
                var content = JsonSerializer.Serialize(addressResolution);
                return new HttpResponseMessage(OK)
                {
                    Content = new StringContent(content)
                };
            });
            var httpClient = new HttpClient(messageHandler)
            {
                BaseAddress = new Uri("http://localhost")
            };
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(x => x.CreateClient(nameof(AvalaraAddressService))).Returns(httpClient);
            var logger = new Mock<ILogger<AvalaraAddressService>>();
            var service = new AvalaraAddressService(httpClientFactory.Object, logger.Object);
            var address = new Address
            {
                Country = "US"
            };

            // Act
            var response = await service.ValidateAsync(address).ConfigureAwait(false);

            // Assert
            var result = Assert.Single(response);
            Assert.Equal(addressResolution.ValidatedAddresses[0].Country, result?.Country);
            logger.Verify(
                x => x.Log(
                    Information,
                    It.Is<EventId>(y => y.Id == (int)ValidateStart && y.Name == $"{ValidateStart}"),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
            logger.Verify(
                x => x.Log(
                    Information,
                    It.Is<EventId>(y => y.Id == (int)ValidateEnd && y.Name == $"{ValidateEnd}"),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task ValidateAsyncLogsError()
        {
            // Arrange
            var httpClient = new HttpClient();
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(x => x.CreateClient(nameof(AvalaraAddressService))).Returns(httpClient);
            var logger = new Mock<ILogger<AvalaraAddressService>>();
            var service = new AvalaraAddressService(httpClientFactory.Object, logger.Object);
            var address = new Address
            {
                Country = "US"
            };
            async Task TestCode() => await service.ValidateAsync(address).ConfigureAwait(false);

            // Act / Assert
            var exception = await Assert.ThrowsAsync<UriFormatException>(TestCode).ConfigureAwait(false);
            logger.Verify(
                x => x.Log(
                    Information,
                    It.Is<EventId>(y => y.Id == (int)ValidateStart && y.Name == $"{ValidateStart}"),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
            logger.Verify(
                x => x.Log(
                    Error,
                    It.Is<EventId>(y => y.Id == (int)ValidateError && y.Name == $"{ValidateError}"),
                    It.IsAny<It.IsAnyType>(),
                    exception,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}
