namespace Services.Models
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;
    using JetBrains.Annotations;

    /// <summary>Represents a validated address.</summary>
    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/ValidatedAddressInfo/
    [ExcludeFromCodeCoverage]
    [PublicAPI]
    public class ValidatedAddressInfo : AddressInfo
    {
        /// <summary>
        /// Gets or sets the address type code. One of:
        /// <list type="bullet">
        /// <item>
        /// <description>Firm or company address</description>
        /// </item>
        /// <item>
        /// <description>General Delivery address</description>
        /// </item>
        /// <item>
        /// <description>High-rise or business complex</description>
        /// </item>
        /// <item>
        /// <description>PO Box address</description>
        /// </item>
        /// <item>
        /// <description>Rural route address</description>
        /// </item>
        /// <item>
        /// <description>Street or residential address</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <value>The type of the address.</value>
        [JsonPropertyName("addressType")]
        public string AddressType { get; set; }
    }
}
