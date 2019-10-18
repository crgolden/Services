namespace Services.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;
    using Enums;

    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/AddressResolutionModel/
    [ExcludeFromCodeCoverage]
    public class AddressResolutionModel
    {
        [JsonPropertyName("address")]
        public AddressInfo? Address { get; set; }

        [JsonPropertyName("validatedAddresses")]
        public IList<ValidatedAddressInfo> ValidatedAddresses { get; set; } = new List<ValidatedAddressInfo>();

        [JsonPropertyName("coordinates")]
        public CoordinateInfo? Coordinates { get; set; }

        [JsonPropertyName("resolutionQuality")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ResolutionQuality ResolutionQuality { get; set; }

        [JsonPropertyName("taxAuthorities")]
        public IList<TaxAuthorityInfo> TaxAuthorities { get; set; } = new List<TaxAuthorityInfo>();

        [JsonPropertyName("messages")]
        public IList<AvaTaxMessage> Messages { get; set; } = new List<AvaTaxMessage>();
    }
}
