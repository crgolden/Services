namespace Services.Address.Avalara.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Logging;
    using Models;
    using Moq;
    using Xunit;
    using static System.Net.HttpStatusCode;
    using static System.Net.Mime.MediaTypeNames.Application;
    using static System.Text.Encoding;
    using static System.Text.Json.JsonSerializer;
    using static Common.EventId;
    using static Microsoft.Extensions.Logging.LogLevel;
    using static Moq.Mock;
    using static Moq.Times;

    public class AvalaraAddressServiceTests
    {
        [Fact]
        public void ThrowsForNullHttpClientFactory()
        {
            // Arrange
            var logger = Of<ILogger<AvalaraAddressService>>();
            object TestCode() => new AvalaraAddressService(default, logger);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("httpClientFactory", exception.ParamName);
        }

        [Fact]
        public void ThrowsForNullLogger()
        {
            // Arrange
            var httpClientFactory = Of<IHttpClientFactory>();
            object TestCode() => new AvalaraAddressService(httpClientFactory, default);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("logger", exception.ParamName);
        }

        [Fact]
        public async Task ValidateAsyncThrowsForNullAddress()
        {
            // Arrange
            var httpClientFactory = Of<IHttpClientFactory>();
            var logger = Of<ILogger<AvalaraAddressService>>();
            var service = new AvalaraAddressService(httpClientFactory, logger);
            Task TestCode() => service.ValidateAsync(default);

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
            var messageHandler = new TestMessageHandler(req =>
            {
                var content = Serialize(addressResolution);
                return new HttpResponseMessage(OK)
                {
                    Content = new StringContent(content, UTF8, Json)
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
            logger.As<ILogger>().Verify(AddressValidateStart.IsLoggedWith(Information), Once);
            logger.As<ILogger>().Verify(AddressValidateEnd.IsLoggedWith(Information), Once);
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
            Task TestCode() => service.ValidateAsync(address);

            // Act / Assert
            var exception = await Assert.ThrowsAsync<UriFormatException>(TestCode).ConfigureAwait(false);
            logger.As<ILogger>().Verify(AddressValidateStart.IsLoggedWith(Information), Once);
            logger.As<ILogger>().Verify(AddressValidateError.IsLoggedWith(Error, exception), Once);
        }
    }
}
