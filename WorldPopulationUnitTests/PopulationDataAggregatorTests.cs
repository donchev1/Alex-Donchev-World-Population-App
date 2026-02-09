using Microsoft.Extensions.Logging;
using Moq;
using WorldPopulationMVC.Data.DTO;
using WorldPopulationMVC.Services;
using WorldPopulationMVC.Services.Interfaces;

namespace WorldPopulationUnitTests;

[TestFixture]
public class PopulationDataAggregatorTests
{
    private Mock<ILogger<IPopulationDataAggregator>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<IPopulationDataAggregator>>();
    }

    private Mock<IPopulationDataProvider> CreateProvider(int rank, string providerName, IEnumerable<CountryPopulationData>? data = null, Exception? toThrow = null)
    {
        var mock = new Mock<IPopulationDataProvider>();
        mock.SetupGet(x => x.Rank).Returns(rank);
        mock.SetupGet(x => x.ProviderName).Returns(providerName);

        if (toThrow is not null)
        {
            mock.Setup(x => x.GetCountriesWithPopulationData()).ThrowsAsync(toThrow);
        }
        else
        {
            mock.Setup(x => x.GetCountriesWithPopulationData()).ReturnsAsync(data?.ToList() ?? new List<CountryPopulationData>());
        }

        return mock;
    }

    [Test]
    public async Task GetPopulationsData_MergesProviders_HonorsLowerRank()
    {
        // Arrange
        var p1 = CreateProvider(1, "P1", new[]
        {
            new CountryPopulationData { CountryName = "Alpha", Population = 9999, Source = "P1" },
            new CountryPopulationData { CountryName = "Beta", Population = 200, Source = "P1" }
        });

        var p2 = CreateProvider(2, "P2", new[]
        {
            new CountryPopulationData { CountryName = "Alpha", Population = 100, Source = "P2" },
            new CountryPopulationData { CountryName = "Gamma", Population = 300, Source = "P2" }
        });

        var aggregator = new PopulationDataAggregator(new[] { p1.Object, p2.Object }, _loggerMock.Object);

        // Act
        var result = await aggregator.GetPopulationsData();

        // Assert
        Assert.That(result.Select(r => r.CountryName).ToList(), Is.EqualTo(new[] { "Alpha", "Beta", "Gamma" }));

        var alpha = result.First(r => r.CountryName == "Alpha");
        Assert.That(alpha.Population, Is.EqualTo(9999));
        Assert.That(alpha.Source, Is.EqualTo("P1"));
    }

    [Test]
    public async Task GetPopulationsData_ContinuesWhenProviderThrows()
    {
        // Arrange
        var good = CreateProvider(1, "No exception Provider", new[]
            {
                new CountryPopulationData { CountryName = "Record One", Population = 50, Source = "No exception Provider" }
            });

        var bad = CreateProvider(2, "Exception Provider", new[]
            {
                new CountryPopulationData { CountryName = "Record Two", Population = 50, Source = "Exception Provider" }
            },
            toThrow: new InvalidOperationException("boom"));

        var aggregator = new PopulationDataAggregator(new[] { good.Object, bad.Object }, _loggerMock.Object);

        // Act
        var result = (await aggregator.GetPopulationsData()).ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].CountryName, Is.EqualTo("Record One"));
        Assert.That(result[0].Population, Is.EqualTo(50));

        // Verify the aggregator logged an error for the failing provider (generic verification)
        _loggerMock.Verify(
            l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.AtLeastOnce);
    }

    [Test]
    public async Task GetPopulationsData_ReturnsDataSortedByCountryName()
    {
        // Arrange
        var provider = CreateProvider(1, "P", new[]
        {
            new CountryPopulationData { CountryName = "Zimbabwe", Population = 10, Source = "P" },
            new CountryPopulationData { CountryName = "Argentina", Population = 1000, Source = "P" },
            new CountryPopulationData { CountryName = "Belgium", Population = 500, Source = "P" }
        });

        var aggregator = new PopulationDataAggregator(new[] { provider.Object }, _loggerMock.Object);

        // Act
        var result = await aggregator.GetPopulationsData();

        // Assert
        Assert.That(result.Select(r => r.CountryName).ToList(), Is.EqualTo(new[] { "Argentina", "Belgium", "Zimbabwe" }));
    }
}