namespace Services.Models
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;

    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/ValidatedAddressInfo/
    [ExcludeFromCodeCoverage]
    public class ValidatedAddressInfo : AddressInfo
    {
        [JsonPropertyName("addressType")]
        public string? AddressType { get; set; }
    }
}
