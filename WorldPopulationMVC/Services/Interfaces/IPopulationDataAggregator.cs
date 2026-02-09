using WorldPopulationMVC.Data.DTO;

namespace WorldPopulationMVC.Services.Interfaces;

public interface IPopulationDataAggregator
{
    Task<IEnumerable<ICountryPopulationData>> GetPopulationsData();
}
