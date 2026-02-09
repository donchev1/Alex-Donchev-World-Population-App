namespace WorldPopulationMVC.Data.DTO;

public class CountryPopulationData : ICountryPopulationData
{
    public string Source { get; set; }
    public string CountryName { get; set; }
    public long Population { get; set; }
}
