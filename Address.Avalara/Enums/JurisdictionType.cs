namespace Services.Enums
{
    using JetBrains.Annotations;

    /// <summary>The type of jurisdiction referenced by this tax authority.</summary>
    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/enums/TaxAuthorityInfo%20%3E%20jurisdictionType/
    [PublicAPI]
    public enum JurisdictionType
    {
        /// <summary>Country</summary>
        Country,

        /// <summary>State</summary>
        State,

        /// <summary>County</summary>
        County,

        /// <summary>City</summary>
        City,

        /// <summary>Special Tax Jurisdiction</summary>
        Special
    }
}
