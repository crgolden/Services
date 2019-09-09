namespace Services.Models
{
    using Newtonsoft.Json;

    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/CoordinateInfo/
    public class CoordinateInfo
    {
        [JsonProperty("latitude")]
        public double? Latitude { get; set; }

        [JsonProperty("longitude")]
        public double? Longitude { get; set; }
    }
}