namespace Services.Address.Avalara.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using static System.Linq.Enumerable;
    using static System.StringComparison;
    using static Moq.Mock;

    public class ServiceCollectionExtensionTests
    {
        [Fact]
        public void AddAvalaraAddressServiceThrowsForNullConfiguration()
        {
            // Arrange
            var services = new ServiceCollection();
            object TestCode() => services.AddAvalaraAddressService(default);

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCode);
            Assert.Equal("configuration", exception.ParamName);
        }

        [Fact]
        public void AddAvalaraAddressServiceThrowsForMissingSection()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(Empty<KeyValuePair<string, string>>())
                .Build();
            object TestCode() => services.AddAvalaraAddressService(configuration);

            // Act / Assert
            var exception = Assert.Throws<ArgumentException>(TestCode);
            Assert.Contains(
                expectedSubstring: $"{nameof(AvalaraAddressOptions)} section doesn't exist",
                actualString: exception.Message,
                comparisonType: CurrentCulture);
            Assert.Equal("configuration", exception.ParamName);
        }

        [Fact]
        public void AddAvalaraAddressServiceThrowsForInvalidSection()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {
                        $"{nameof(AvalaraAddressOptions)}:{nameof(AvalaraAddressOptions.LicenseKey)}",
                        string.Empty
                    }
                })
                .Build();
            var services = new ServiceCollection();
            object TestCode() => services.AddAvalaraAddressService(configuration);

            // Act / Assert
            var exception = Assert.Throws<ArgumentException>(TestCode);
            Assert.Contains(
                expectedSubstring: $"{nameof(AvalaraAddressOptions)} section is invalid",
                actualString: exception.Message,
                comparisonType: CurrentCulture);
            Assert.Equal("configuration", exception.ParamName);
        }

        [Fact]
        public void AddAvalaraAddressService()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {
                        $"{nameof(AvalaraAddressOptions)}:{nameof(AvalaraAddressOptions.LicenseKey)}",
                        "Valid"
                    }
                })
                .Build();
            var services = new ServiceCollection();
            services.AddSingleton(Of<ILogger<AvalaraAddressService>>());

            // Act
            var response = services.AddAvalaraAddressService(configuration);

            // Assert
            using var provider = response.BuildServiceProvider();
            provider.GetRequiredService<IHttpClientFactory>();
            var addressService = provider.GetRequiredService<IAddressService>();
            Assert.IsType<AvalaraAddressService>(addressService);
        }
    }
}
