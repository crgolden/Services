namespace Services.Enums
{
    using JetBrains.Annotations;

    /// <summary>The resolution quality of the geospatial coordinates.</summary>
    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/enums/AddressResolutionModel%20%3E%20resolutionQuality/
    [PublicAPI]
    public enum ResolutionQuality
    {
        /// <summary>Location was not geocoded</summary>
        NotCoded,

        /// <summary>Location was already geocoded on the request</summary>
        External,

        /// <summary>Avalara-defined country centroid</summary>
        CountryCentroid,

        /// <summary>Avalara-defined state / province centroid</summary>
        RegionCentroid,

        /// <summary>Geocoded at a level more coarse than a PostalCentroid</summary>
        PartialCentroid,

        /// <summary>Largest postal code (zip5 in US, left three in CA, etc)</summary>
        PostalCentroidGood,

        /// <summary>Better postal code (zip7 in US)</summary>
        PostalCentroidBetter,

        /// <summary>Best postal code (zip9 in US, complete postal code elsewhere)</summary>
        PostalCentroidBest,

        /// <summary>Nearest intersection</summary>
        Intersection,

        /// <summary>Interpolated to rooftop</summary>
        Interpolated,

        /// <summary>Assumed to be rooftop level, non-interpolated</summary>
        Rooftop,

        /// <summary>Pulled from a static list of geocodes for specific jurisdictions</summary>
        Constant
    }
}
