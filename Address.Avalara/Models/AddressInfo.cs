namespace Services.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;
    using JetBrains.Annotations;

    /// <summary>Represents a base address element.</summary>
    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/AddressInfo/
    [ExcludeFromCodeCoverage]
    [PublicAPI]
    public class AddressInfo
    {
        /// <summary>Gets or sets the first line of the street address.</summary>
        /// <value>The First line of the street address.</value>
        [JsonPropertyName("line1")]
        [MaxLength(50)]
        public string Line1 { get; set; }

        /// <summary>Gets or sets the second line of the street address.</summary>
        /// <value>The second line of the street address.</value>
        [JsonPropertyName("line2")]
        [MaxLength(100)]
        public string Line2 { get; set; }

        /// <summary>Gets or sets the third line of the street address.</summary>
        /// <value>The third line of the street address.</value>
        [JsonPropertyName("line3")]
        [MaxLength(100)]
        public string Line3 { get; set; }

        /// <summary>Gets or sets the city component of the address.</summary>
        /// <value>The city component of the address.</value>
        [JsonPropertyName("city")]
        [MaxLength(50)]
        public string City { get; set; }

        /// <summary>
        /// <para>Gets or sets the name or ISO 3166 code identifying the region within the country.</para>
        /// <para>This field supports many different region identifiers:</para>
        /// <list type="bullet">
        /// <item>
        /// <description>Two and three character ISO 3166 region codes</description>
        /// </item>
        /// <item>
        /// <description>Fully spelled out names of the region in ISO supported languages</description>
        /// </item>
        /// <item>
        /// <description>Common alternative spellings for many regions</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <value>The region.</value>
        [JsonPropertyName("region")]
        public string Region { get; set; }

        /// <summary>
        /// <para>Gets or sets the name or ISO 3166 code identifying the country.</para>
        /// <para>This field supports many different country identifiers:</para>
        /// <list type="bullet">
        /// <item>
        /// <description>Two character ISO 3166 codes</description>
        /// </item>
        /// <item>
        /// <description>Three character ISO 3166 codes</description>
        /// </item>
        /// <item>
        /// <description>Fully spelled out names of the country in ISO supported languages</description>
        /// </item>
        /// <item>
        /// <description>Common alternative spellings for many countries</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <value>The name or ISO 3166 code identifying the country.</value>
        [JsonPropertyName("country")]
        public string Country { get; set; }

        /// <summary>Gets or sets the postal code /zip code component of the address.</summary>
        /// <value>The postal code / zip code component of the address.</value>
        [JsonPropertyName("postalCode")]
        [MaxLength(11)]
        public string PostalCode { get; set; }

        /// <summary>Gets or sets the geospatial latitude measurement, in decimal degrees floating point format.</summary>
        /// <value>The the geospatial latitude measurement.</value>
        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        /// <summary>Gets or sets the geospatial longitude measurement, in decimal degrees floating point format.</summary>
        /// <value>The geospatial longitude measurement.</value>
        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }
    }
}
