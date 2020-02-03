namespace Services
{
    using JetBrains.Annotations;

    /// <summary>Configuration settings for the <see cref="EmailQueueClientService"/> class.</summary>
    [PublicAPI]
    public class EmailQueueClientOptions
    {
        /// <summary>Gets or sets the name of the shared access key.</summary>
        /// <value>The name of the shared access key.</value>
        public string SharedAccessKeyName { get; set; }

        /// <summary>Gets or sets the primary key.</summary>
        /// <value>The primary key.</value>
        public string PrimaryKey { get; set; }

        /// <summary>Gets or sets the secondary key.</summary>
        /// <value>The secondary key.</value>
        public string SecondaryKey { get; set; }

        /// <summary>Gets or sets the endpoint.</summary>
        /// <value>The endpoint.</value>
        public string Endpoint { get; set; }

        /// <summary>Gets or sets the name of the email queue.</summary>
        /// <value>The name of the email queue.</value>
        public string EmailQueueName { get; set; }
    }
}
