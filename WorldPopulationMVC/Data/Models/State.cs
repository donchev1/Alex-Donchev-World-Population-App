namespace WorldPopulationMVC.Data.Models;

public class State
{
    public long StateId { get; set; }
    public string StateName { get; set; }
    public long CountryId { get; set; }
    public Country Country { get; set; }
    public ICollection<City> Cities {  get; set; }
}
