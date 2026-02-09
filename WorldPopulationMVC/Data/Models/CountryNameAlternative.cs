namespace WorldPopulationMVC.Data.Models;

public class CountryNameAlternative
{
    public long Id { get; set; }
    public string Name { get; set; }
    public long CountryNameId { get; set; }
    public virtual CountryName DefaultName { get; set; }
}
