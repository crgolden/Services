namespace Services.Models
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;
    using Enums;

    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/TaxAuthorityInfo/
    [ExcludeFromCodeCoverage]
    public class TaxAuthorityInfo
    {
        [JsonPropertyName("avalaraId")]
        public string? AvalaraId { get; set; }

        [JsonPropertyName("jurisdictionName")]
        public string? JurisdictionName { get; set; }

        [JsonPropertyName("jurisdictionType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JurisdictionType JurisdictionType { get; set; }

        [JsonPropertyName("signatureCode")]
        public string? SignatureCode { get; set; }
    }
}
