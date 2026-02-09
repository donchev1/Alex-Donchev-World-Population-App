using Microsoft.EntityFrameworkCore;
using WorldPopulationMVC.Data.Repositories.Interfaces;

namespace WorldPopulationMVC.Data.Repositories;

public class CountryNameRepository : ICountryNameRepository
{
    private readonly AppDbContext _appDbContext;
    private Dictionary<string, string> _countriesNames;
    public CountryNameRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<string> GetDefaultName(string name)
    {
        await LoadAll();

        if (string.IsNullOrWhiteSpace(name))
            return name;

        if (_countriesNames.TryGetValue(name, out var defaultName))
            return defaultName;
        else
            return name;
    }

    private async Task LoadAll()
    {
        if (_countriesNames != null)
            return;

        _countriesNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        //We have registered this repository as scoped so each time we ping the api it will load the names only once,
        //even if it is called from multiple classes this saves us trips to the db.
        //It is not singleton because I don't want to keep all the data in memory all the time 
        //and also if we add a name to the db we don't have to restart the service.
        //Getting the entire table from the db is not good practice, but we know that this data set 
        //won't be too big, we might have 1000 records here tops. So in this case I think it is ok. 
        var countries = await _appDbContext.CountryNames.Include(x => x.Alternatives).ToListAsync();

        //we load every unique name (default names + alternatives) as key to the dictionary and the default name as a value
        //this way when we look for a particular name's default value we have the fastest possible lookup O(1)
        foreach (var country in countries)
        {
            // default name → itself
            _countriesNames[country.Name] = country.Name;

            // alternative names → default
            if (country.Alternatives == null) continue;

            foreach (var alt in country.Alternatives)
            {
                _countriesNames[alt.Name] = country.Name;
            }
        }
    }

}
