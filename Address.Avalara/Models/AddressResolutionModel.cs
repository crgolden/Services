namespace Services.Models
{
    using Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/AddressResolutionModel/
    public class AddressResolutionModel
    {
        [JsonProperty("address")]
        public AddressInfo Address { get; set; }

        [JsonProperty("validatedAddresses")]
        public ValidatedAddressInfo[] ValidatedAddresses { get; set; }

        [JsonProperty("coordinates")]
        public CoordinateInfo Coordinates { get; set; }

        [JsonProperty("resolutionQuality")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ResolutionQuality ResolutionQuality { get; set; }

        [JsonProperty("taxAuthorities")]
        public TaxAuthorityInfo[] TaxAuthorities { get; set; }

        [JsonProperty("messages")]
        public AvaTaxMessage[] Messages { get; set; }
    }
}
