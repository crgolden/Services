﻿namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Logging;
    using SmartyStreets;
    using static System.DateTime;
    using static Common.EventId;
    using static Microsoft.Extensions.Logging.LogLevel;
    using EventId = Microsoft.Extensions.Logging.EventId;
    using InternationalLookup = SmartyStreets.InternationalStreetApi.Lookup;
    using UsLookup = SmartyStreets.USStreetApi.Lookup;

    public class SmartyStreetsAddressService : IAddressService
    {
        private readonly IClient<UsLookup> _usClient;
        private readonly IClient<InternationalLookup> _internationalClient;
        private readonly ILogger<SmartyStreetsAddressService> _logger;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:Field names should not use Hungarian notation", Justification = "Lowercased US")]
        public SmartyStreetsAddressService(
            IClient<UsLookup>? usClient,
            IClient<InternationalLookup>? internationalClient,
            ILogger<SmartyStreetsAddressService>? logger)
        {
            _usClient = usClient ?? throw new ArgumentNullException(nameof(usClient));
            _internationalClient = internationalClient ?? throw new ArgumentNullException(nameof(internationalClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<IEnumerable<Address>> ValidateAsync(
            Address? address,
            CancellationToken cancellationToken = default)
        {
            if (address == default)
            {
                throw new ArgumentNullException(nameof(address));
            }

            try
            {
                _logger.Log(
                    logLevel: Information,
                    eventId: new EventId((int)ValidateStart, $"{ValidateStart}"),
                    message: "Validating address {@Address} at {@Time}",
                    args: new object[] { address, UtcNow });
                var addresses = new[] { "US", "USA", "CA", "CAN" }.Contains(address.Country)
                    ? ValidateUsAddress(address)
                    : ValidateInternationalAddress(address);
                _logger.Log(
                    logLevel: Information,
                    eventId: new EventId((int)ValidateEnd, $"{ValidateEnd}"),
                    message: "Validated address {@Address} with result {@Addresses} at {@Time}",
                    args: new object[] { address, addresses, UtcNow });
                return Task.FromResult(addresses);
            }
            catch (Exception e)
            {
                _logger.Log(
                    logLevel: Error,
                    eventId: new EventId((int)ValidateError, $"{ValidateError}"),
                    exception: e,
                    message: "Error validating address {@Address} at {@Time}",
                    args: new object[] { address, UtcNow });
                throw;
            }
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
                ? Enumerable.Empty<Address>()
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
                ? Enumerable.Empty<Address>()
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
