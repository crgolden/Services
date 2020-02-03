namespace Services.Models
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;
    using JetBrains.Annotations;

    /// <summary>Coordinate Info.</summary>
    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/CoordinateInfo/
    [ExcludeFromCodeCoverage]
    [PublicAPI]
    public class CoordinateInfo
    {
        /// <summary>Gets or sets the latitude.</summary>
        /// <value>The latitude.</value>
        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        /// <summary>Gets or sets the longitude.</summary>
        /// <value>The longitude.</value>
        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }
    }
}