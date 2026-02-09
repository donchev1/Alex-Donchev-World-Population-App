using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WorldPopulationMVC.APIs.CountryPopulation;
using WorldPopulationMVC.Configuration;
using WorldPopulationMVC.Data.Repositories.Interfaces;
using WorldPopulationMVC.Services;

namespace WorldPopulationUnitTests;

[TestFixture]
public class StatServiceDataManagerTests
{
    private Mock<IStatService> _statServiceMock;
    private Mock<ICountryNameRepository> _countryNameRepositoryMock;
    private Mock<ILogger<StatServiceDataManager>> _loggerMock;
    private IOptions<StatServiceDataManagerConfig> _options;

    [SetUp]
    public void Setup()
    {
        _statServiceMock = new Mock<IStatService>();
        _countryNameRepositoryMock = new Mock<ICountryNameRepository>();
        _loggerMock = new Mock<ILogger<StatServiceDataManager>>();
        _options = Options.Create(new StatServiceDataManagerConfig { Rank = 1 });
    }

    [Test]
    public async Task GetCountriesWithPopulationData_WhenStatServiceThrows_LogsAndRethrows()
    {
        // Arrange
        var ex = new InvalidOperationException("service failure");
        _statServiceMock.Setup(s => s.GetCountryPopulationsAsync()).ThrowsAsync(ex);

        var manager = new StatServiceDataManager(
            _statServiceMock.Object,
            _countryNameRepositoryMock.Object,
            _options,
            _loggerMock.Object);

        // Act / Assert
        var thrown = Assert.ThrowsAsync<InvalidOperationException>(async () => await manager.GetCountriesWithPopulationData());
        Assert.That(thrown, Is.SameAs(ex));

        // Verify an error log was emitted
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.AtLeastOnce);
    }

    [Test]
    public async Task GetCountriesWithPopulationData_MapsNameWithParentheses_ToDefaultName()
    {
        // Arrange
        var firstRecordPopulation = 1000;

        var apiData = new List<Tuple<string, int>>
        {
            Tuple.Create("United States (USA)", firstRecordPopulation)
        };

        _statServiceMock.Setup(s => s.GetCountryPopulationsAsync()).ReturnsAsync(apiData);
        _countryNameRepositoryMock.Setup(r => r.GetDefaultName("USA")).ReturnsAsync("United States of America");

        var manager = new StatServiceDataManager(
            _statServiceMock.Object,
            _countryNameRepositoryMock.Object,
            _options,
            _loggerMock.Object);

        // Act
        var result = (await manager.GetCountriesWithPopulationData()).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        var item = result.Single();
        Assert.That(item.CountryName, Is.EqualTo("United States of America"));
        Assert.That(item.Population, Is.EqualTo(firstRecordPopulation));
        Assert.That(item.Source, Is.EqualTo("Stat Service API"));
    }

    [Test]
    public async Task GetCountriesWithPopulationData_CompleteRecordOverwritesPreviousIncompleteRecord()
    {
        // Arrange
        var firstRecordPopulation = 100;
        var secondRecordPopulation = 250;
        var apiData = new List<Tuple<string, int>>
        {
            Tuple.Create("Test City (France)", firstRecordPopulation),
            Tuple.Create("France", secondRecordPopulation)
        };

        _statServiceMock.Setup(s => s.GetCountryPopulationsAsync()).ReturnsAsync(apiData);
        _countryNameRepositoryMock.Setup(r => r.GetDefaultName("France")).ReturnsAsync("France");

        var manager = new StatServiceDataManager(
            _statServiceMock.Object,
            _countryNameRepositoryMock.Object,
            _options,
            _loggerMock.Object);

        // Act
        var result = (await manager.GetCountriesWithPopulationData()).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        var item = result.Single();
        Assert.That(item.CountryName, Is.EqualTo("France"));
        Assert.That(item.Population, Is.EqualTo(secondRecordPopulation));
    }

    [Test]
    public async Task GetCountriesWithPopulationData_IncompleteRecordsAccumulate()
    {
        // Arrange
        var firstRecordPopulation = 100;
        var secondRecordPopulation = 250;

        var apiData = new List<Tuple<string, int>>
        {
            Tuple.Create("Test City (France)", firstRecordPopulation),
            Tuple.Create("Another Test City (France)", secondRecordPopulation)
        };

        _statServiceMock.Setup(s => s.GetCountryPopulationsAsync()).ReturnsAsync(apiData);
        _countryNameRepositoryMock.Setup(r => r.GetDefaultName("France")).ReturnsAsync("France");

        var manager = new StatServiceDataManager(
            _statServiceMock.Object,
            _countryNameRepositoryMock.Object,
            _options,
            _loggerMock.Object);

        // Act
        var result = (await manager.GetCountriesWithPopulationData()).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        var item = result.Single();
        Assert.That(item.CountryName, Is.EqualTo("France"));
        Assert.That(item.Population, Is.EqualTo(firstRecordPopulation + secondRecordPopulation));
    }

    [Test]
    public async Task GetCountriesWithPopulationData_IncompleteRecordsDontOverwriteCompleteRecords()
    {
        // Arrange

        var firstRecordPopulation = 100;
        var secondRecordPopulation = 250;
        var countryName = "Complete Country";

        var apiData = new List<Tuple<string, int>>
        {
            Tuple.Create(countryName, secondRecordPopulation),
            Tuple.Create($"Incomplete City ({countryName})", firstRecordPopulation),
        };

        _statServiceMock.Setup(s => s.GetCountryPopulationsAsync()).ReturnsAsync(apiData);
        _countryNameRepositoryMock.Setup(r => r.GetDefaultName(
            It.Is<string>(s => s == countryName || s == $"Incomplete City ({countryName})")))
            .ReturnsAsync(countryName);

        var manager = new StatServiceDataManager(
            _statServiceMock.Object,
            _countryNameRepositoryMock.Object,
            _options,
            _loggerMock.Object);

        // Act
        var result = (await manager.GetCountriesWithPopulationData()).ToList();

        // Assert: second complete record should overwrite the first (population becomes 250)
        Assert.That(result, Has.Count.EqualTo(1));
        var item = result.Single();
        Assert.That(item.CountryName, Is.EqualTo(countryName));
        Assert.That(item.Population, Is.EqualTo(secondRecordPopulation));
    }
}