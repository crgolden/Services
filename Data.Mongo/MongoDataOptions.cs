namespace Services
{
    using System.Collections.Generic;

    public class MongoDataOptions
    {
        public string? ConnectionString { get; set; }

        public string? DatabaseName { get; set; }

        public IDictionary<string, string> CollectionNames { get; set; } = new Dictionary<string, string>();
    }
}
