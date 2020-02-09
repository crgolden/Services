namespace Services.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;
    using Enums;
    using JetBrains.Annotations;

    /// <summary>Address Resolution Model.</summary>
    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/AddressResolutionModel/
    [ExcludeFromCodeCoverage]
    [PublicAPI]
    public class AddressResolutionModel
    {
        /// <summary>Gets or sets the original address.</summary>
        /// <value>The original address.</value>
        [JsonPropertyName("address")]
        public AddressInfo Address { get; set; }

        /// <summary>Gets or sets the validated address or addresses.</summary>
        /// <value>The validated address or addresses.</value>
        // https://github.com/dotnet/runtime/issues/30258
        [JsonPropertyName("validatedAddresses")]
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Open Github issue")]
        public IList<ValidatedAddressInfo> ValidatedAddresses { get; set; } = new List<ValidatedAddressInfo>();

        /// <summary>Gets or sets the geospatial coordinates of this address.</summary>
        /// <value>The geospatial coordinates of this address.</value>
        [JsonPropertyName("coordinates")]
        public CoordinateInfo Coordinates { get; set; }

        /// <summary>Gets or sets the resolution quality of the geospatial coordinates.</summary>
        /// <value>The resolution quality of the geospatial coordinates.</value>
        [JsonPropertyName("resolutionQuality")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ResolutionQuality ResolutionQuality { get; set; }

        /// <summary>Gets the tax authorities.</summary>
        /// <value>The tax authorities.</value>
        [JsonPropertyName("taxAuthorities")]
        public IList<TaxAuthorityInfo> TaxAuthorities { get; } = new List<TaxAuthorityInfo>();

        /// <summary>Gets the informational and warning messages regarding this address.</summary>
        /// <value>The informational and warning messages regarding this address.</value>
        [JsonPropertyName("messages")]
        public IList<AvaTaxMessage> Messages { get; } = new List<AvaTaxMessage>();
    }
}
