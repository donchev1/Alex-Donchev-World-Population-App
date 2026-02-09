using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WorldPopulationMVC.Models;
using WorldPopulationMVC.Services.Interfaces;

namespace WorldPopulationMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPopulationDataAggregator _populationDataAggregator;

    public HomeController(ILogger<HomeController> logger, IPopulationDataAggregator populationDataAggregator)
    {
        _logger = logger;
        _populationDataAggregator = populationDataAggregator;
    }

    public async Task<IActionResult> CountryPopulationData()
    {
        try
        {
            var populationData = await _populationDataAggregator.GetPopulationsData();
            return View(populationData);
        }
        catch (Exception ex)
        {
            _logger.LogError("Aggregating population data failed.", ex);
            throw;
        }
        
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
