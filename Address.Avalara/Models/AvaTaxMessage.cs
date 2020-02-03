namespace Services.Models
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;
    using JetBrains.Annotations;

    /// <summary>Informational or warning messages returned by AvaTax with a transaction.</summary>
    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/AvaTaxMessage/
    [ExcludeFromCodeCoverage]
    [PublicAPI]
    public class AvaTaxMessage
    {
        /// <summary>Gets or sets the brief summary of what this message tells us.</summary>
        /// <value>The brief summary of what this message tells us.</value>
        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        /// <summary>Gets or sets the detailed information that explains what the summary provided.</summary>
        /// <value>The detailed information that explains what the summary provided.</value>
        [JsonPropertyName("details")]
        public string Details { get; set; }

        /// <summary>Gets or sets the information about what object in your request this message refers to.</summary>
        /// <value>The information about what object in your request this message refers to.</value>
        [JsonPropertyName("refersTo")]
        public string RefersTo { get; set; }

        /// <summary>Gets or sets the category that indicates how severely this message affects the results.</summary>
        /// <value>The category that indicates how severely this message affects the results.</value>
        [JsonPropertyName("severity")]
        public string Severity { get; set; }

        /// <summary>Gets or sets the name of the code or service that generated this message.</summary>
        /// <value>The name of the code or service that generated this message.</value>
        [JsonPropertyName("source")]
        public string Source { get; set; }
    }
}
