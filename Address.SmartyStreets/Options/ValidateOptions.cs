﻿namespace Services.Options
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;
    using static System.String;
    using static Microsoft.Extensions.Options.ValidateOptionsResult;

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used implicitly")]
    internal class ValidateOptions : IValidateOptions<SmartyStreetsAddressOptions>
    {
        public ValidateOptionsResult Validate(string name, SmartyStreetsAddressOptions options)
        {
            return IsNullOrWhiteSpace(options.AuthId) ||
                   IsNullOrWhiteSpace(options.AuthToken)
                ? Fail($"'{nameof(SmartyStreetsAddressOptions)}' section is invalid")
                : Success;
        }
    }
}
