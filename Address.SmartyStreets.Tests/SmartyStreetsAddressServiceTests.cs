namespace Services.Address.SmartyStreets.Tests
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using global::SmartyStreets;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using static Common.EventId;
    using static Microsoft.Extensions.Logging.LogLevel;
    using EventId = Microsoft.Extensions.Logging.EventId;
    using InternationalCandidate = global::SmartyStreets.InternationalStreetApi.Candidate;
    using InternationalComponents = global::SmartyStreets.InternationalStreetApi.Components;
    using InternationalLookup = global::SmartyStreets.InternationalStreetApi.Lookup;
    using UsCandidate = global::SmartyStreets.USStreetApi.Candidate;
    using UsComponents = global::SmartyStreets.USStreetApi.Components;
    using UsLookup = global::SmartyStreets.USStreetApi.Lookup;

    public class SmartyStreetsAddressServiceTests
    {
        [Fact]
        public void ThrowsForNullUsClient()
        {
            // Arrange
            var internationalClient = Mock.Of<IClient<InternationalLookup>>();
            var logger = Mock.Of<ILogger<SmartyStreetsAddressService>>();
            object TestCode() => new SmartyStreetsAddressService(default, internationalClient, logger);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("usClient", exception.ParamName);
        }

        [Fact]
        public void ThrowsForNullInternationalClient()
        {
            // Arrange
            var usClient = Mock.Of<IClient<UsLookup>>();
            var logger = Mock.Of<ILogger<SmartyStreetsAddressService>>();
            object TestCode() => new SmartyStreetsAddressService(usClient, default, logger);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("internationalClient", exception.ParamName);
        }

        [Fact]
        public void ThrowsForNullLogger()
        {
            // Arrange
            var usClient = Mock.Of<IClient<UsLookup>>();
            var internationalClient = Mock.Of<IClient<InternationalLookup>>();
            object TestCode() => new SmartyStreetsAddressService(usClient, internationalClient, default);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("logger", exception.ParamName);
        }

        [Fact]
        public async Task ValidateAsyncThrowsForNullAddress()
        {
            // Arrange
            var usClient = Mock.Of<IClient<UsLookup>>();
            var internationalClient = Mock.Of<IClient<InternationalLookup>>();
            var logger = Mock.Of<ILogger<SmartyStreetsAddressService>>();
            var service = new SmartyStreetsAddressService(usClient, internationalClient, logger);
            async Task TestCode() => await service.ValidateAsync(default).ConfigureAwait(false);

            // Act // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
            Assert.Equal("address", exception.ParamName);
        }

        [Fact]
        public async Task ValidateAsyncUsAddress()
        {
            // Arrange
            var address = new Address
            {
                StreetAddress = "Street Address",
                Locality = "Locality",
                Region = "Region",
                PostalCode = "Postal Code",
                Country = "USA"
            };
            var candidate = new UsCandidate
            {
                Components = new UsComponents
                {
                    StreetName = $"Candidate {address.StreetAddress}",
                    CityName = $"Candidate {address.Locality}",
                    State = $"Candidate {address.Region}",
                    ZipCode = $"Candidate {address.PostalCode}"
                }
            };
            var usClient = new Mock<IClient<UsLookup>>();
            usClient
                .Setup(x => x.Send(It.Is<UsLookup>(y => y.Street == address.StreetAddress &&
                                                        y.City == address.Locality &&
                                                        y.State == address.Region &&
                                                        y.ZipCode == address.PostalCode)))
                .Callback<UsLookup>(x => x.AddToResult(candidate));
            var internationalClient = new Mock<IClient<InternationalLookup>>();
            var logger = new Mock<ILogger<SmartyStreetsAddressService>>();
            var service = new SmartyStreetsAddressService(usClient.Object, internationalClient.Object, logger.Object);

            // Act
            var response = await service.ValidateAsync(address).ConfigureAwait(false);

            // Assert
            var result = Assert.Single(response);
            Assert.Equal(candidate.Components.StreetName, result?.StreetAddress);
            Assert.Equal(candidate.Components.CityName, result?.Locality);
            Assert.Equal(candidate.Components.State, result?.Region);
            Assert.Equal(candidate.Components.ZipCode, result?.PostalCode);
            internationalClient.Verify(x => x.Send(It.IsAny<InternationalLookup>()), Times.Never);
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
        public async Task ValidateAsyncInternationalAddress()
        {
            // Arrange
            var address = new Address
            {
                StreetAddress = "Street Address",
                Locality = "Locality",
                Region = "Region",
                PostalCode = "Postal Code",
                Country = "CHN"
            };
            var candidate = new InternationalCandidate
            {
                Components = new InternationalComponents
                {
                    ThoroughfareName = $"Candidate {address.StreetAddress}",
                    Locality = $"Candidate {address.Locality}",
                    AdministrativeArea = $"Candidate {address.Region}",
                    PostalCode = $"Candidate {address.PostalCode}",
                    CountryIso3 = $"Candidate {address.Country}"
                }
            };
            var usClient = new Mock<IClient<UsLookup>>();
            var internationalClient = new Mock<IClient<InternationalLookup>>();
            internationalClient
                .Setup(x => x.Send(It.Is<InternationalLookup>(y => y.Address1 == address.StreetAddress &&
                                                                   y.Locality == address.Locality &&
                                                                   y.AdministrativeArea == address.Region &&
                                                                   y.PostalCode == address.PostalCode &&
                                                                   y.Country == address.Country)))
                .Callback<InternationalLookup>(x => x.AddToResult(candidate));
            var logger = new Mock<ILogger<SmartyStreetsAddressService>>();
            var service = new SmartyStreetsAddressService(usClient.Object, internationalClient.Object, logger.Object);

            // Act
            var response = await service.ValidateAsync(address).ConfigureAwait(false);

            // Assert
            var result = Assert.Single(response);
            Assert.Equal(candidate.Components.ThoroughfareName, result?.StreetAddress);
            Assert.Equal(candidate.Components.Locality, result?.Locality);
            Assert.Equal(candidate.Components.AdministrativeArea, result?.Region);
            Assert.Equal(candidate.Components.PostalCode, result?.PostalCode);
            Assert.Equal(candidate.Components.CountryIso3, result?.Country);
            usClient.Verify(x => x.Send(It.IsAny<UsLookup>()), Times.Never);
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
            var address = new Address
            {
                Country = "USA"
            };
            var usClient = new Mock<IClient<UsLookup>>();
            usClient.Setup(x => x.Send(It.IsAny<UsLookup>())).Throws<ArgumentNullException>();
            var internationalClient = Mock.Of<IClient<InternationalLookup>>();
            var logger = new Mock<ILogger<SmartyStreetsAddressService>>();
            var service = new SmartyStreetsAddressService(usClient.Object, internationalClient, logger.Object);
            async Task TestCode() => await service.ValidateAsync(address).ConfigureAwait(false);

            // Act / Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(TestCode).ConfigureAwait(false);
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
