namespace Services
{
    using JetBrains.Annotations;

    /// <summary>Configuration settings for the <see cref="SmartyStreetsAddressOptions"/> class.</summary>
    [PublicAPI]
    public class SmartyStreetsAddressOptions
    {
        /// <summary>Gets or sets the authentication identifier.</summary>
        /// <value>The authentication identifier.</value>
        public string AuthId { get; set; }

        /// <summary>Gets or sets the authentication token.</summary>
        /// <value>The authentication token.</value>
        public string AuthToken { get; set; }
    }
}
