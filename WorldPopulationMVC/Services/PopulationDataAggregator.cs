using WorldPopulationMVC.Data.DTO;
using WorldPopulationMVC.Services.Interfaces;

namespace WorldPopulationMVC.Services;

public class PopulationDataAggregator : IPopulationDataAggregator
{
    private IEnumerable<IPopulationDataProvider> _dataProviders;
    private ILogger<IPopulationDataAggregator> _logger;
    public PopulationDataAggregator(IEnumerable<IPopulationDataProvider> dataProviders, 
        ILogger<IPopulationDataAggregator> logger)
    {
        _dataProviders = dataProviders;
        _logger = logger;
    }

    public async Task<IEnumerable<ICountryPopulationData>> GetPopulationsData()
    {
        Dictionary<string, ICountryPopulationData> populationsData = new();

        var tasks = _dataProviders
        .OrderBy(x => x.Rank)
        .Select(async provider =>
        {
            //we use this try-catch here because if getting the data from one provider fails we still want to 
            //use the data from others.
            try
            {
                return await provider.GetCountriesWithPopulationData();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error while fetching data from provider: {provider.ProviderName}.",ex);
                return new List<CountryPopulationData>();
            }
        })
        .ToList();

        //This approach is the fastest, but it loads all the data from all data providers in memory.
        //As we have only 2 providers and we are limited in terms of data set size (we only have about 200 countries worldwide)
        //this is totally fine, but in a scenario of more or larger data sets another algorithm should be considered.
        var results = await Task.WhenAll(tasks);

        foreach (var countriesPopulationData in results)
        {
            foreach (var country in countriesPopulationData)
            {
                populationsData.TryAdd(country.CountryName, country);
            }
        }

        return populationsData.Select(x => x.Value).OrderBy(x=> x.CountryName).ToList();
    }
}
