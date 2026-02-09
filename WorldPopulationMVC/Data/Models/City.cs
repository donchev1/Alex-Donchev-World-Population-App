namespace WorldPopulationMVC.Data.Models;

public class City
{
    public long CityId { get; set; }
    public string CityName { get; set; }
    public long StateId { get; set; }
    public long? Population { get; set; }
    public virtual State State { get; set; }

}
