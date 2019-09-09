namespace Services.Models
{
    using Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/TaxAuthorityInfo/
    public class TaxAuthorityInfo
    {
        [JsonProperty("avalaraId")]
        public string AvalaraId { get; set; }

        [JsonProperty("jurisdictionName")]
        public string JurisdictionName { get; set; }

        [JsonProperty("jurisdictionType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public JurisdictionType JurisdictionType { get; set; }

        [JsonProperty("signatureCode")]
        public string SignatureCode { get; set; }
    }
}
