namespace WorldPopulationMVC.Data.Models;

public class CountryName
{
    public long Id { get; set; }
    public string Name { get; set; }
    public ICollection<CountryNameAlternative> Alternatives { get; set; }
}
