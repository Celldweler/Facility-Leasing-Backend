using FluentAssertions;
using Leasing.Api.Controllers;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Leasing.Api.UnitTests.Controllers;

public class WeatherForecastControllerTests
{
    private ILogger<WeatherForecastController> logger;
    private WeatherForecastController controller;

    [SetUp]
    public void Setup()
    {
        logger = Substitute.For<ILogger<WeatherForecastController>>();
        controller = new WeatherForecastController(logger);
    }

    [Test]
    public void GetListOfWeatherForecasts()
    {
        // Act
        var result = controller.Get();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IEnumerable<WeatherForecast>>();
    }
}