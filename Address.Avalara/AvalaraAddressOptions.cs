namespace Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;

    /// <summary>Configuration settings for the <see cref="AvalaraAddressService"/> class.</summary><summary></summary>
    [PublicAPI]
    public class AvalaraAddressOptions
    {
        /// <summary>Gets or sets the license key.</summary>
        /// <value>The license key.</value>
        public string LicenseKey { get; set; }

        /// <summary>Gets or sets the base address.</summary>
        /// <value>The base address.</value>
        [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "Default sandbox")]
        public Uri BaseAddress { get; set; } = new Uri("https://sandbox-rest.avatax.com/api/v2");
    }
}
