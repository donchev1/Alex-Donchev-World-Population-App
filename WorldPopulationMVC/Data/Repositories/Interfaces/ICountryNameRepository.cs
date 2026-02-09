namespace WorldPopulationMVC.Data.Repositories.Interfaces;

public interface ICountryNameRepository
{
    Task<string> GetDefaultName(string name);
}
