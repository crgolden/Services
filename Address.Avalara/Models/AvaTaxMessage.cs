namespace Services.Models
{
    using Newtonsoft.Json;

    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/AvaTaxMessage/
    public class AvaTaxMessage
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }

        [JsonProperty("refersTo")]
        public string RefersTo { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }
    }
}
