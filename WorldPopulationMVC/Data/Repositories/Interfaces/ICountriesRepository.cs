using WorldPopulationMVC.Data.DTO;

namespace WorldPopulationMVC.Data.Repositories.Interfaces;

public interface ICountriesRepository
{
    Task<List<CountryPopulationData>> GetCountriesWithPopulationData();
}
