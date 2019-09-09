namespace Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Options;
    using SmartyStreets;
    using InternationalLookup = SmartyStreets.InternationalStreetApi.Lookup;
    using UsLookup = SmartyStreets.USStreetApi.Lookup;

    public class SmartyStreetsAddressService : IAddressService
    {
        private readonly ClientBuilder _clientBuilder;

        public SmartyStreetsAddressService(IOptions<SmartyStreetsAddressOptions> addressOptions)
        {
            _clientBuilder = new ClientBuilder(
                authId: addressOptions.Value.AuthId,
                authToken: addressOptions.Value.AuthToken);
        }

        public Task<IEnumerable<Address>> ValidateAsync(
            Address address,
            CancellationToken cancellationToken = default)
        {
            return new[] { "US", "USA", "CA", "CAN" }.Contains(address.Country)
                ? Task.FromResult(ValidateUsAddress(address))
                : Task.FromResult(ValidateInternationalAddress(address));
        }

        private IEnumerable<Address> ValidateUsAddress(Address address)
        {
            var client = _clientBuilder.BuildUsStreetApiClient();
            var lookup = new UsLookup
            {
                Street = address.StreetAddress,
                City = address.Locality,
                State = address.Region,
                ZipCode = address.PostalCode
            };
            client.Send(lookup);
            return lookup.Result.Select(candidate => new Address
            {
                StreetAddress = candidate.Components.StreetName,
                Locality = candidate.Components.CityName,
                Region = candidate.Components.State,
                PostalCode = candidate.Components.ZipCode
            });
        }

        private IEnumerable<Address> ValidateInternationalAddress(Address address)
        {
            var client = _clientBuilder.BuildInternationalStreetApiClient();
            var lookup = new InternationalLookup
            {
                Address1 = address.StreetAddress,
                Locality = address.Locality,
                AdministrativeArea = address.Region,
                PostalCode = address.PostalCode,
                Country = address.Country
            };
            client.Send(lookup);
            return lookup.Result.Select(candidate => new Address
            {
                StreetAddress = candidate.Components.ThoroughfareName,
                Locality = candidate.Components.Locality,
                Region = candidate.Components.AdministrativeArea,
                PostalCode = candidate.Components.PostalCode,
                Country = candidate.Components.CountryIso3
            });
        }
    }
}
