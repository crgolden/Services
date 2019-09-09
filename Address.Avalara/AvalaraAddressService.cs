namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Options;
    using Models;
    using Newtonsoft.Json;

    public class AvalaraAddressService : IAddressService
    {
        private readonly HttpClient _httpClient;

        public AvalaraAddressService(
            IOptions<AvalaraAddressOptions> addressOptions,
            IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(AvalaraAddressService));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                scheme: "Basic",
                parameter: addressOptions.Value.LicenseKey);
            _httpClient.BaseAddress = new Uri(addressOptions.Value.BaseAddress);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IEnumerable<Address>> ValidateAsync(
            Address address,
            CancellationToken cancellationToken = default)
        {
            AddressResolutionModel addressResolution;
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
                addressResolution = JsonConvert.DeserializeObject<AddressResolutionModel>(responseString);
            }

            return addressResolution.ValidatedAddresses.Select(validatedAddress => new Address
            {
                StreetAddress = validatedAddress.Line1,
                Locality = validatedAddress.City,
                Region = validatedAddress.Region,
                PostalCode = validatedAddress.PostalCode,
                Country = validatedAddress.Country
            });
        }
    }
}
