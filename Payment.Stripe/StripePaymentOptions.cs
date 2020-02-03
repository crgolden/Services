namespace Services
{
    using JetBrains.Annotations;

    /// <summary>Configuration settings for the <see cref="StripePaymentService"/> class.</summary>
    [PublicAPI]
    public class StripePaymentOptions
    {
        /// <summary>Gets or sets the secret key.</summary>
        /// <value>The secret key.</value>
        public string SecretKey { get; set; }
    }
}
