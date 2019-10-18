namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Logging;
    using Models;
    using static System.DateTime;
    using static Common.EventId;
    using EventId = Microsoft.Extensions.Logging.EventId;

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

        public async Task<IEnumerable<Address>> ValidateAsync(
            Address? address,
            CancellationToken cancellationToken = default)
        {
            if (address == default)
            {
                throw new ArgumentNullException(nameof(address));
            }

            try
            {
                AddressResolutionModel addressResolution;
                _logger.LogInformation(
                    eventId: new EventId((int)ValidateStart, $"{ValidateStart}"),
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
                    addressResolution = JsonSerializer.Deserialize<AddressResolutionModel>(responseString);
                }

                var addresses = addressResolution?.ValidatedAddresses == null
                    ? Enumerable.Empty<Address>()
                    : addressResolution.ValidatedAddresses.Select(validatedAddress => new Address
                    {
                        StreetAddress = $"{validatedAddress.Line1} {validatedAddress.Line2} {validatedAddress.Line3}".TrimEnd(),
                        Locality = validatedAddress.City,
                        Region = validatedAddress.Region,
                        PostalCode = validatedAddress.PostalCode,
                        Country = validatedAddress.Country
                    });
                _logger.LogInformation(
                    eventId: new EventId((int)ValidateEnd, $"{ValidateEnd}"),
                    message: "Validated address {@Address} with result {@Addresses} at {@Time}",
                    args: new object[] { address, addresses, UtcNow });
                return addresses;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    eventId: new EventId((int)ValidateError, $"{ValidateError}"),
                    exception: e,
                    message: "Error validating address {@Address} at {@Time}",
                    args: new object[] { address, UtcNow });
                throw;
            }
        }
    }
}
