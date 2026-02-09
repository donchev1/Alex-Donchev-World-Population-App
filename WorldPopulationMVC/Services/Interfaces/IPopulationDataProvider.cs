using WorldPopulationMVC.Data.DTO;

namespace WorldPopulationMVC.Services.Interfaces;

public interface IPopulationDataProvider
{
    public Task<IEnumerable<ICountryPopulationData>> GetCountriesWithPopulationData();
    public int Rank { get; }
    public string ProviderName { get; }

}
