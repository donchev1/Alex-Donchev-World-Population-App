using Microsoft.EntityFrameworkCore;
using WorldPopulationMVC.Data.DTO;
using WorldPopulationMVC.Data.Repositories.Interfaces;

namespace WorldPopulationMVC.Data.Repositories;

public class CountriesRepository : ICountriesRepository
{
    private readonly AppDbContext _context;

    public CountriesRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<CountryPopulationData>> GetCountriesWithPopulationData()
    {
        return await _context.Countries
            .Where(c => !string.IsNullOrWhiteSpace(c.CountryName))
            .Select(c =>
        new CountryPopulationData
        {
            CountryName = c.CountryName,
            Population = c.States.SelectMany(s=> s.Cities)
            .Sum(c => (long?)c.Population ?? 0)
        }).ToListAsync();
    }
}
