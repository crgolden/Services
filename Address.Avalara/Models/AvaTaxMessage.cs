namespace Services.Models
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;

    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/AvaTaxMessage/
    [ExcludeFromCodeCoverage]
    public class AvaTaxMessage
    {
        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("details")]
        public string? Details { get; set; }

        [JsonPropertyName("refersTo")]
        public string? RefersTo { get; set; }

        [JsonPropertyName("severity")]
        public string? Severity { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }
    }
}
