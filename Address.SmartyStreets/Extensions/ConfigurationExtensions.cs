﻿namespace Microsoft.Extensions.Configuration
{
    using System;
    using JetBrains.Annotations;
    using Services;

    /// <summary>A class with methods that extend <see cref="IConfiguration"/>.</summary>
    [PublicAPI]
    public static class ConfigurationExtensions
    {
        /// <summary>Gets the <see cref="SmartyStreetsAddressOptions"/> configuration section.</summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance.</param>
        /// <returns>The <see cref="SmartyStreetsAddressOptions"/> configuration section.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">The <see cref="SmartyStreetsAddressOptions"/> configuration section doesn't exist.</exception>
        public static IConfigurationSection GetSmartyStreetsAddressOptionsSection(this IConfiguration configuration)
        {
            if (configuration == default)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection(nameof(SmartyStreetsAddressOptions));
            if (!section.Exists())
            {
                throw new ArgumentException($"'{nameof(SmartyStreetsAddressOptions)}' section doesn't exist", nameof(configuration));
            }

            return section;
        }
    }
}
