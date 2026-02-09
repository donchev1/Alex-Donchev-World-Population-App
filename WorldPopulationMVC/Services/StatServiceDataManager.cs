using Microsoft.Extensions.Options;
using WorldPopulationMVC.APIs.CountryPopulation;
using WorldPopulationMVC.Configuration;
using WorldPopulationMVC.Data.DTO;
using WorldPopulationMVC.Data.Repositories.Interfaces;
using WorldPopulationMVC.Services.Interfaces;

namespace WorldPopulationMVC.Services;

public class StatServiceDataManager : IPopulationDataProvider
{
    public string ProviderName => "Stat Service API";
    public int Rank { get; set; }

    private readonly IStatService _statService;
    private readonly ICountryNameRepository _countryNameRepository;
    private readonly ILogger<StatServiceDataManager> _logger;

    public StatServiceDataManager(IStatService statService,
        ICountryNameRepository countryNameRepository,
        IOptions<StatServiceDataManagerConfig> config,
        ILogger<StatServiceDataManager> logger)
    {
        Rank = config.Value.Rank;
        _statService = statService;
        _countryNameRepository = countryNameRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ICountryPopulationData>> GetCountriesWithPopulationData()
    {
        List<Tuple<string, int>> apiData;

        try
        {
            apiData = await _statService.GetCountryPopulationsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Stat Service API Call failed.", ex);
            throw;
        }

        Dictionary<string, StatServiceData> result = new();

        foreach (var countryPopulationInfo in apiData)
        {
            var rawName = countryPopulationInfo.Item1;
            var currentRecordPopulation = countryPopulationInfo.Item2;
            var nameCleaned = CleanName(countryPopulationInfo.Item1);
            var defaultName = await _countryNameRepository.GetDefaultName(nameCleaned);
            var currentRecordDataComplete = nameCleaned == rawName;

            //I consider "U.S. Virgin Islands (US)" as an incomplete record, because it doesn't carry information for the entire country
            //just for the Virgin Islands population. And I consider a record like "United States of America" for complete beucase
            //the population data is for the entire country
            if (!string.IsNullOrEmpty(defaultName))
            {
                if (result.TryGetValue(defaultName, out var savedRecord))
                {
                    if (savedRecord.DataIsComplete)
                        continue;
                    
                    if (currentRecordDataComplete)
                    {
                        //we replace incomplete data with complete data
                        savedRecord.Population = currentRecordPopulation;
                    }
                    else
                    {
                        //in this case both records are incomplete so we sum
                        savedRecord.Population += currentRecordPopulation;
                    }
                       
                }
                else
                {
                    result.Add(defaultName, new StatServiceData
                    {
                        CountryName = defaultName,
                        Population = currentRecordPopulation,
                        Source = ProviderName,
                        DataIsComplete = currentRecordDataComplete
                    });
                }
            }
        }

        return result.Select(x=> x.Value);
    }

    private string CleanName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return name;

        if (name.Contains('(') && name.Contains('('))
        {
            int start = name.IndexOf('(');
            int end = name.IndexOf(')');

            if (start >= 0 && end > start)
            {
                return name.Substring(start + 1, end - start - 1).Trim();
            }

            return name.Trim();
        }
        else
        {
            return name.Trim();
        }
    }

}
