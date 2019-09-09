namespace Services.Models.Enums
{
    // https://developer.avalara.com/api-reference/avatax/rest/v2/models/enums/AddressResolutionModel%20%3E%20resolutionQuality/
    public enum ResolutionQuality
    {
        NotCoded,
        External,
        CountryCentroid,
        RegionCentroid,
        PartialCentroid,
        PostalCentroidGood,
        PostalCentroidBetter,
        PostalCentroidBest,
        Intersection,
        Interpolated,
        Rooftop,
        Constant,
    }
}
