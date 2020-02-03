namespace Services
{
    using JetBrains.Annotations;

    /// <summary>Configuration settings for the <see cref="SendGridEmailService"/> class.</summary>
    [PublicAPI]
    public class SendGridEmailOptions
    {
        /// <summary>Gets or sets the API key.</summary>
        /// <value>The API key.</value>
        public string ApiKey { get; set; }
    }
}
