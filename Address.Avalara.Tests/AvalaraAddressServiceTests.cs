namespace Services.Address.Avalara.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common;
    using Models;
    using Moq;
    using Xunit;
    using static System.Net.HttpStatusCode;
    using static System.Net.Mime.MediaTypeNames.Application;
    using static System.Text.Encoding;
    using static System.Text.Json.JsonSerializer;
    using static Moq.Mock;

    public class AvalaraAddressServiceTests
    {
        [Fact]
        public void ThrowsForNullHttpClientFactory()
        {
            // Arrange
            AvalaraAddressService TestCode() => new AvalaraAddressService(default);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("httpClientFactory", exception.ParamName);
        }

        [Fact]
        public void ThrowsForNullLogger()
        {
            // Arrange
            AvalaraAddressService TestCode() => new AvalaraAddressService(Of<IHttpClientFactory>());

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("logger", exception.ParamName);
        }

        [Fact]
        public async Task ValidateAsyncThrowsForNullAddress()
        {
            // Arrange
            Task TestCode() => new AvalaraAddressService(Of<IHttpClientFactory>()).ValidateAsync(default);

            // Act // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(true);
            Assert.Equal("address", exception.ParamName);
        }

        [Fact]
        public async Task ValidateAsync()
        {
            // Arrange
            var addressResolution = new AddressResolutionModel();
            addressResolution.ValidatedAddresses.Add(new ValidatedAddressInfo
            {
                Country = "US"
            });
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
            var service = new AvalaraAddressService(httpClientFactory.Object);
            var address = new Address
            {
                Country = "US"
            };

            // Act
            var response = await service.ValidateAsync(address).ConfigureAwait(true);

            // Assert
            var result = Assert.Single(response);
            Assert.Equal(addressResolution.ValidatedAddresses[0].Country, result?.Country);
        }
    }
}
