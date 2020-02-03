namespace Services.Models
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;
    using Enums;
    using JetBrains.Annotations;

    /// <summary>Information about a tax authority relevant for an address.</summary>
    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/TaxAuthorityInfo/
    [ExcludeFromCodeCoverage]
    [PublicAPI]
    public class TaxAuthorityInfo
    {
        /// <summary>Gets or sets the unique ID number assigned by Avalara to this tax authority.</summary>
        /// <value>The unique ID number assigned by Avalara to this tax authority.</value>
        [JsonPropertyName("avalaraId")]
        public string AvalaraId { get; set; }

        /// <summary>Gets or sets the friendly jurisdiction name for this tax authority.</summary>
        /// <value>The friendly jurisdiction name for this tax authority.</value>
        [JsonPropertyName("jurisdictionName")]
        public string JurisdictionName { get; set; }

        /// <summary>Gets or sets the type of jurisdiction referenced by this tax authority.</summary>
        /// <value>The type of jurisdiction referenced by this tax authority.</value>
        [JsonPropertyName("jurisdictionType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JurisdictionType JurisdictionType { get; set; }

        /// <summary>Gets or sets the Avalara-assigned signature code for this tax authority.</summary>
        /// <value>The Avalara-assigned signature code for this tax authority.</value>
        [JsonPropertyName("signatureCode")]
        public string SignatureCode { get; set; }
    }
}
