namespace Services.Models
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;

    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/CoordinateInfo/
    [ExcludeFromCodeCoverage]
    public class CoordinateInfo
    {
        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }
    }
}