namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Logging;
    using Models;
    using static System.DateTime;
    using static System.Linq.Enumerable;
    using static System.Text.Json.JsonSerializer;
    using static Common.EventId;
    using static Microsoft.Extensions.Logging.LogLevel;
    using EventId = Microsoft.Extensions.Logging.EventId;

    /// <inheritdoc />
    public class AvalaraAddressService : IAddressService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AvalaraAddressService> _logger;

        public AvalaraAddressService(
            IHttpClientFactory? httpClientFactory,
            ILogger<AvalaraAddressService>? logger)
        {
            if (httpClientFactory == default)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            _httpClient = httpClientFactory.CreateClient(nameof(AvalaraAddressService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public Task<IEnumerable<Address>> ValidateAsync(
            Address? address,
            LogLevel logLevel = Information,
            CancellationToken cancellationToken = default)
        {
            if (address == default)
            {
                throw new ArgumentNullException(nameof(address));
            }

            return Validate(address, logLevel, cancellationToken);
        }

        private async Task<IEnumerable<Address>> Validate(
            Address address,
            LogLevel logLevel,
            CancellationToken cancellationToken)
        {
            try
            {
                AddressResolutionModel addressResolution;
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)AddressValidateStart, $"{AddressValidateStart}"),
                    message: "Validating address {@Address} at {@Time}",
                    args: new object[] { address, UtcNow });
                var stringBuilder = new StringBuilder($"{_httpClient.BaseAddress}/addresses/resolve");
                stringBuilder.Append($"?line1={address.StreetAddress}");
                stringBuilder.Append($"&city={address.Locality}");
                stringBuilder.Append($"&region={address.Region}");
                stringBuilder.Append($"&postalCode={address.PostalCode}");
                stringBuilder.Append($"&country={address.Country}");
                var requestUri = new Uri($"{stringBuilder}");
                using (var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    addressResolution = Deserialize<AddressResolutionModel>(responseString);
                }

                var addresses = addressResolution?.ValidatedAddresses == null
                    ? Empty<Address>()
                    : addressResolution.ValidatedAddresses.Select(validatedAddress => new Address
                    {
                        StreetAddress = $"{validatedAddress.Line1} {validatedAddress.Line2} {validatedAddress.Line3}".TrimEnd(),
                        Locality = validatedAddress.City,
                        Region = validatedAddress.Region,
                        PostalCode = validatedAddress.PostalCode,
                        Country = validatedAddress.Country
                    });
                _logger.Log(
                    logLevel: logLevel,
                    eventId: new EventId((int)AddressValidateEnd, $"{AddressValidateEnd}"),
                    message: "Validated address {@Address} with result {@Addresses} at {@Time}",
                    args: new object[] { address, addresses, UtcNow });
                return addresses;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    eventId: new EventId((int)AddressValidateError, $"{AddressValidateError}"),
                    exception: e,
                    message: "Error validating address {@Address} at {@Time}",
                    args: new object[] { address, UtcNow });
                throw;
            }
        }
    }
}
