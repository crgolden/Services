namespace Services.Address.Avalara.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Common;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

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
                .AddInMemoryCollection(Enumerable.Empty<KeyValuePair<string, string>>())
                .Build();
            object TestCode() => services.AddAvalaraAddressService(configuration);

            // Act / Assert
            var exception = Assert.Throws<Exception>(TestCode);
            Assert.Equal($"{nameof(AvalaraAddressOptions)} section doesn't exist", exception.Message);
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
            var exception = Assert.Throws<Exception>(TestCode);
            Assert.Equal($"{nameof(AvalaraAddressOptions)} section is invalid", exception.Message);
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
            services.AddSingleton(Mock.Of<ILogger<AvalaraAddressService>>());

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
