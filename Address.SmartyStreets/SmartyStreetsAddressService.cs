namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Common.Services;
    using JetBrains.Annotations;
    using SmartyStreets;
    using static System.Linq.Enumerable;
    using static System.String;
    using static System.Threading.Tasks.Task;
    using InternationalLookup = SmartyStreets.InternationalStreetApi.Lookup;
    using UsLookup = SmartyStreets.USStreetApi.Lookup;

    /// <inheritdoc />
    [PublicAPI]
    public class SmartyStreetsAddressService : IAddressService
    {
        private readonly IClient<UsLookup> _usClient;
        private readonly IClient<InternationalLookup> _internationalClient;

        /// <summary>Initializes a new instance of the <see cref="SmartyStreetsAddressService"/> class.</summary>
        /// <param name="usClient">The us client.</param>
        /// <param name="internationalClient">The international client.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="usClient"/> is <see langword="null"/>
        /// or
        /// <paramref name="internationalClient"/> is <see langword="null"/>
        /// or
        /// <paramref name="name"/> is <see langword="null"/>.</exception>
        public SmartyStreetsAddressService(
            IClient<UsLookup> usClient,
            IClient<InternationalLookup> internationalClient,
            string name = nameof(SmartyStreetsAddressService))
        {
            if (IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            _usClient = usClient ?? throw new ArgumentNullException(nameof(usClient));
            _internationalClient = internationalClient ?? throw new ArgumentNullException(nameof(internationalClient));
            Name = name;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Task<IEnumerable<Address>> ValidateAsync(Address address, CancellationToken cancellationToken = default)
        {
            if (address == default)
            {
                throw new ArgumentNullException(nameof(address));
            }

            var addresses = new[] { "US", "USA", "CA", "CAN" }.Contains(address.Country)
                ? ValidateUsAddress(address)
                : ValidateInternationalAddress(address);
            return FromResult(addresses);
        }

        private IEnumerable<Address> ValidateUsAddress(Address address)
        {
            var lookup = new UsLookup
            {
                Street = address.StreetAddress,
                City = address.Locality,
                State = address.Region,
                ZipCode = address.PostalCode
            };
            _usClient.Send(lookup);
            return lookup.Result == default
                ? Empty<Address>()
                : lookup.Result.Where(x => x.Components != default).Select(x => new Address
                {
                    StreetAddress = x.Components.StreetName,
                    Locality = x.Components.CityName,
                    Region = x.Components.State,
                    PostalCode = x.Components.ZipCode
                });
        }

        private IEnumerable<Address> ValidateInternationalAddress(Address address)
        {
            var lookup = new InternationalLookup
            {
                Address1 = address.StreetAddress,
                Locality = address.Locality,
                AdministrativeArea = address.Region,
                PostalCode = address.PostalCode,
                Country = address.Country
            };
            _internationalClient.Send(lookup);
            return lookup.Result == default
                ? Empty<Address>()
                : lookup.Result.Where(x => x.Components != default).Select(x => new Address
                {
                    StreetAddress = x.Components.ThoroughfareName,
                    Locality = x.Components.Locality,
                    Region = x.Components.AdministrativeArea,
                    PostalCode = x.Components.PostalCode,
                    Country = x.Components.CountryIso3
                });
        }
    }
}
