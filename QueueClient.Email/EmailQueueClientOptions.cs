namespace Services
{
    public class EmailQueueClientOptions
    {
        public string? SharedAccessKeyName { get; set; }

        public string? PrimaryKey { get; set; }

        public string? SecondaryKey { get; set; }

        public string? Endpoint { get; set; }

        public string? EmailQueueName { get; set; }
    }
}
