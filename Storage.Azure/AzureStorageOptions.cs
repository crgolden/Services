namespace Services
{
    using JetBrains.Annotations;

    /// <summary>Configuration settings for the <see cref="AzureStorageService"/> class.</summary>
    [PublicAPI]
    public class AzureStorageOptions
    {
        /// <summary>Gets or sets the name of the account.</summary>
        /// <value>The name of the account.</value>
        public string AccountName { get; set; }

        /// <summary>Gets or sets the first account key.</summary>
        /// <value>The first account key.</value>
        public string AccountKey1 { get; set; }

        /// <summary>Gets or sets the second account key.</summary>
        /// <value>The second account key.</value>
        public string AccountKey2 { get; set; }
    }
}
