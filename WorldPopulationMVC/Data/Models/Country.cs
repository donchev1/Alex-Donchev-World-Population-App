namespace WorldPopulationMVC.Data.Models;

public class Country
{
    public long CountryId { get; set; }
    public string CountryName { get; set; }

    public ICollection<State> States { get; set; }
}
