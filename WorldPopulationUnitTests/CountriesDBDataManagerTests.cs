using Microsoft.Extensions.Options;
using Moq;
using WorldPopulationMVC.Configuration;
using WorldPopulationMVC.Data.DTO;
using WorldPopulationMVC.Data.Repositories.Interfaces;
using WorldPopulationMVC.Services;

namespace WorldPopulationUnitTests;

[TestFixture]
public class CountriesDBDataManagerTests
{
    private Mock<ICountriesRepository> _countriesRepositoryMock;
    private Mock<ICountryNameRepository> _countryNameRepositoryMock;
    private IOptions<CountriesDBDataManagerConfig> _options;

    [SetUp]
    public void Setup()
    {
        _countriesRepositoryMock = new Mock<ICountriesRepository>();
        _countryNameRepositoryMock = new Mock<ICountryNameRepository>();
        _options = Options.Create(new CountriesDBDataManagerConfig { Rank = 42 });
    }

    [Test]
    public async Task Rank_Is_Populated_From_Config()
    {
        // Arrange
        var manager = new CountriesDBDataManager(_countriesRepositoryMock.Object, _countryNameRepositoryMock.Object, _options);

        // Act / Assert
        Assert.That(manager.Rank, Is.EqualTo(42));
    }

    [Test]
    public async Task GetCountriesWithPopulationData_ResolvesNamesAndSetsSource()
    {
        // Arrange
        var repoList = new List<CountryPopulationData>
        {
            new CountryPopulationData { CountryName = "france", Population = 10, Source = "orig" },
            new CountryPopulationData { CountryName = "spain", Population = 20, Source = "orig" }
        };

        _countriesRepositoryMock
            .Setup(r => r.GetCountriesWithPopulationData())
            .ReturnsAsync(repoList);

        _countryNameRepositoryMock
            .Setup(r => r.GetDefaultName("france"))
            .ReturnsAsync("France");

        _countryNameRepositoryMock
            .Setup(r => r.GetDefaultName("spain"))
            .ReturnsAsync("Spain");

        var manager = new CountriesDBDataManager(_countriesRepositoryMock.Object, _countryNameRepositoryMock.Object, _options);

        // Act
        var result = (await manager.GetCountriesWithPopulationData()).ToList();

        // Assert: returned list same length and names resolved, source set to provider name, population preserved
        Assert.That(result, Has.Count.EqualTo(2));

        Assert.That(result[0].CountryName, Is.EqualTo("France"));
        Assert.That(result[0].Population, Is.EqualTo(10));
        Assert.That(result[0].Source, Is.EqualTo("Countries Database"));

        Assert.That(result[1].CountryName, Is.EqualTo("Spain"));
        Assert.That(result[1].Population, Is.EqualTo(20));
        Assert.That(result[1].Source, Is.EqualTo("Countries Database"));

        _countriesRepositoryMock.Verify(r => r.GetCountriesWithPopulationData(), Times.Once);
        _countryNameRepositoryMock.Verify(r => r.GetDefaultName("france"), Times.Once);
        _countryNameRepositoryMock.Verify(r => r.GetDefaultName("spain"), Times.Once);
    }
}