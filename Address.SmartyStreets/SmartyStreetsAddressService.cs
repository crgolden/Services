namespace Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Logging;
    using SmartyStreets;

    public class SmartyStreetsAddressService : IAddressService
    {
        private readonly IClient<SmartyStreets.USStreetApi.Lookup> _usClient;
        private readonly IClient<SmartyStreets.InternationalStreetApi.Lookup> _internationalClient;
        private readonly ILogger<SmartyStreetsAddressService> _logger;

        public SmartyStreetsAddressService(
            IClient<SmartyStreets.USStreetApi.Lookup> usClient,
            IClient<SmartyStreets.InternationalStreetApi.Lookup> internationalClient,
            ILogger<SmartyStreetsAddressService> logger)
        {
            _usClient = usClient;
            _internationalClient = internationalClient;
            _logger = logger;
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
            var lookup = new SmartyStreets.USStreetApi.Lookup
            {
                Street = address.StreetAddress,
                City = address.Locality,
                State = address.Region,
                ZipCode = address.PostalCode
            };
            _usClient.Send(lookup);
            return lookup.Result?.Select(candidate => new Address
            {
                StreetAddress = candidate.Components.StreetName,
                Locality = candidate.Components.CityName,
                Region = candidate.Components.State,
                PostalCode = candidate.Components.ZipCode
            });
        }

        private IEnumerable<Address> ValidateInternationalAddress(Address address)
        {
            var lookup = new SmartyStreets.InternationalStreetApi.Lookup
            {
                Address1 = address.StreetAddress,
                Locality = address.Locality,
                AdministrativeArea = address.Region,
                PostalCode = address.PostalCode,
                Country = address.Country
            };
            _internationalClient.Send(lookup);
            return lookup.Result?.Select(candidate => new Address
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
