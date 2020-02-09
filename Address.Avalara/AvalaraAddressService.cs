namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Models;
    using static System.Linq.Enumerable;
    using static System.Text.Json.JsonSerializer;

    /// <inheritdoc />
    public class AvalaraAddressService : IAddressService
    {
        private readonly HttpClient _httpClient;

        /// <summary>Initializes a new instance of the <see cref="AvalaraAddressService"/> class.</summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <exception cref="ArgumentNullException"><paramref name="httpClientFactory"/> is <see langword="null"/>.</exception>
        public AvalaraAddressService(IHttpClientFactory httpClientFactory)
        {
            if (httpClientFactory == default)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            _httpClient = httpClientFactory.CreateClient(nameof(AvalaraAddressService));
        }

        /// <inheritdoc />
        public Task<IEnumerable<Address>> ValidateAsync(Address address, CancellationToken cancellationToken = default)
        {
            if (address == default)
            {
                throw new ArgumentNullException(nameof(address));
            }

            async Task<IEnumerable<Address>> ValidateAsync()
            {
                AddressResolutionModel result;
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
                    var body = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    result = await DeserializeAsync<AddressResolutionModel>(body, default, cancellationToken).ConfigureAwait(false);
                }

                var addresses = result?.ValidatedAddresses == null
                    ? Empty<Address>()
                    : result.ValidatedAddresses.Select(validatedAddress => new Address
                    {
                        StreetAddress = $"{validatedAddress.Line1} {validatedAddress.Line2} {validatedAddress.Line3}".TrimEnd(),
                        Locality = validatedAddress.City,
                        Region = validatedAddress.Region,
                        PostalCode = validatedAddress.PostalCode,
                        Country = validatedAddress.Country
                    });
                return addresses;
            }

            return ValidateAsync();
        }
    }
}
