namespace Services.Address.SmartyStreets.Tests
{
    using System;
    using System.Collections.Generic;
    using Common;
    using global::SmartyStreets;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using static System.Linq.Enumerable;
    using static System.StringComparison;
    using static Moq.Mock;
    using static Xunit.Assert;
    using InternationalClient = global::SmartyStreets.InternationalStreetApi.Client;
    using InternationalLookup = global::SmartyStreets.InternationalStreetApi.Lookup;
    using UsClient = global::SmartyStreets.USStreetApi.Client;
    using UsLookup = global::SmartyStreets.USStreetApi.Lookup;

    public class ServiceCollectionExtensionTests
    {
        [Fact]
        public void AddSmartyStreetsAddressServiceThrowsForNullConfiguration()
        {
            // Arrange
            var services = new ServiceCollection();
            object TestCode() => services.AddSmartyStreetsAddressService(default);

            // Act / Assert
            var exception = Throws<ArgumentNullException>(TestCode);
            Equal("configuration", exception.ParamName);
        }

        [Fact]
        public void AddSmartyStreetsAddressServiceThrowsForMissingSection()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(Empty<KeyValuePair<string, string>>())
                .Build();
            object TestCode() => services.AddSmartyStreetsAddressService(configuration);

            // Act / Assert
            var exception = Throws<ArgumentException>(TestCode);
            Contains($"{nameof(SmartyStreetsAddressOptions)} section doesn't exist", exception.Message, CurrentCulture);
            Equal("configuration", exception.ParamName);
        }

        [Theory]
        [InlineData("", "Valid")]
        [InlineData("Valid", "")]
        public void AddSmartyStreetsAddressServiceThrowsForInvalidSection(string authId, string authToken)
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {
                        $"{nameof(SmartyStreetsAddressOptions)}:{nameof(SmartyStreetsAddressOptions.AuthId)}",
                        authId
                    },
                    {
                        $"{nameof(SmartyStreetsAddressOptions)}:{nameof(SmartyStreetsAddressOptions.AuthToken)}",
                        authToken
                    }
                })
                .Build();
            var services = new ServiceCollection();
            object TestCode() => services.AddSmartyStreetsAddressService(configuration);

            // Act / Assert
            var exception = Throws<ArgumentException>(TestCode);
            Contains($"{nameof(SmartyStreetsAddressOptions)} section is invalid", exception.Message, CurrentCulture);
            Equal("configuration", exception.ParamName);
        }

        [Fact]
        public void AddSmartyStreetsAddressService()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {
                        $"{nameof(SmartyStreetsAddressOptions)}:{nameof(SmartyStreetsAddressOptions.AuthId)}",
                        "Valid"
                    },
                    {
                        $"{nameof(SmartyStreetsAddressOptions)}:{nameof(SmartyStreetsAddressOptions.AuthToken)}",
                        "Valid"
                    }
                })
                .Build();
            var services = new ServiceCollection();
            services.AddSingleton(Of<ILogger<SmartyStreetsAddressService>>());

            // Act
            var response = services.AddSmartyStreetsAddressService(configuration);

            // Assert
            using var provider = response.BuildServiceProvider();
            var usClient = provider.GetRequiredService<IClient<UsLookup>>();
            IsType<UsClient>(usClient);
            var internationalClient = provider.GetRequiredService<IClient<InternationalLookup>>();
            IsType<InternationalClient>(internationalClient);
            var addressService = provider.GetRequiredService<IAddressService>();
            IsType<SmartyStreetsAddressService>(addressService);
        }
    }
}
