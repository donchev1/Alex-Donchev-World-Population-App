namespace WorldPopulationMVC.Data.DTO;

public interface ICountryPopulationData
{
    public string Source { get; set; }
    public string CountryName { get; set; }
    public long Population { get; set; }
}
