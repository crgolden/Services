namespace Services.Address.SmartyStreets.Tests
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using global::SmartyStreets;
    using Moq;
    using Xunit;
    using static Moq.Mock;
    using static Moq.Times;
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
            object TestCode() => new SmartyStreetsAddressService(default, Of<IClient<InternationalLookup>>());

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("usClient", exception.ParamName);
        }

        [Fact]
        public void ThrowsForNullInternationalClient()
        {
            // Arrange
            object TestCode() => new SmartyStreetsAddressService(Of<IClient<UsLookup>>(), default);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("internationalClient", exception.ParamName);
        }

        [Fact]
        public async Task ValidateAsyncThrowsForNullAddress()
        {
            // Arrange
            Task TestCode() => new SmartyStreetsAddressService(
                Of<IClient<UsLookup>>(),
                Of<IClient<InternationalLookup>>()
            ).ValidateAsync(default);

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
                .Setup(x => x.Send(It.Is<UsLookup>(y => y.Street == address.StreetAddress &
                                                        y.City == address.Locality &&
                                                        y.State == address.Region &&
                                                        y.ZipCode == address.PostalCode)))
                .Callback<UsLookup>(x => x.AddToResult(candidate));
            var internationalClient = new Mock<IClient<InternationalLookup>>();
            var service = new SmartyStreetsAddressService(usClient.Object, internationalClient.Object);

            // Act
            var response = await service.ValidateAsync(address).ConfigureAwait(false);

            // Assert
            var result = Assert.Single(response);
            Assert.Equal(candidate.Components.StreetName, result?.StreetAddress);
            Assert.Equal(candidate.Components.CityName, result?.Locality);
            Assert.Equal(candidate.Components.State, result?.Region);
            Assert.Equal(candidate.Components.ZipCode, result?.PostalCode);
            internationalClient.Verify(x => x.Send(It.IsAny<InternationalLookup>()), Never);
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
            var service = new SmartyStreetsAddressService(usClient.Object, internationalClient.Object);

            // Act
            var response = await service.ValidateAsync(address).ConfigureAwait(false);

            // Assert
            var result = Assert.Single(response);
            Assert.Equal(candidate.Components.ThoroughfareName, result?.StreetAddress);
            Assert.Equal(candidate.Components.Locality, result?.Locality);
            Assert.Equal(candidate.Components.AdministrativeArea, result?.Region);
            Assert.Equal(candidate.Components.PostalCode, result?.PostalCode);
            Assert.Equal(candidate.Components.CountryIso3, result?.Country);
            usClient.Verify(x => x.Send(It.IsAny<UsLookup>()), Never);
        }
    }
}
