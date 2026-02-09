using Microsoft.Extensions.Options;
using WorldPopulationMVC.Configuration;
using WorldPopulationMVC.Data.DTO;
using WorldPopulationMVC.Data.Repositories.Interfaces;
using WorldPopulationMVC.Services.Interfaces;

namespace WorldPopulationMVC.Services;

public class CountriesDBDataManager : IPopulationDataProvider
{
    public string ProviderName => "Countries Database";
    public int Rank { get; set; }

    private ICountriesRepository _countriesRepository;
    private readonly ICountryNameRepository _countryNameRepository;

    public CountriesDBDataManager(ICountriesRepository countriesRepository, 
        ICountryNameRepository countryNameRepository,
        IOptions<CountriesDBDataManagerConfig> config)
    {
        _countriesRepository = countriesRepository;
        _countryNameRepository = countryNameRepository;
        Rank = config.Value.Rank;
    }

    public async Task<IEnumerable<ICountryPopulationData>> GetCountriesWithPopulationData()
    {
        var countriesData = await _countriesRepository.GetCountriesWithPopulationData();

        foreach (var country in countriesData)
        {
            country.CountryName = await _countryNameRepository.GetDefaultName(country.CountryName);
            country.Source = ProviderName;
        }

        return countriesData;
    }
}
