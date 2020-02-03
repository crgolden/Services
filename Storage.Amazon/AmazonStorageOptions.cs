namespace Services
{
    using JetBrains.Annotations;

    /// <summary>Configuration settings for the <see cref="AmazonStorageService"/> class.</summary>
    [PublicAPI]
    public class AmazonStorageOptions
    {
        /// <summary>Gets or sets the access key identifier.</summary>
        /// <value>The access key identifier.</value>
        public string AccessKeyId { get; set; }

        /// <summary>Gets or sets the secret access key.</summary>
        /// <value>The secret access key.</value>
        public string SecretAccessKey { get; set; }
    }
}
