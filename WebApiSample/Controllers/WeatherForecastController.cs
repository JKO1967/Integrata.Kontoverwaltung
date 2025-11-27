using Microsoft.AspNetCore.Mvc;
using WebApiSample.Services;

namespace WebApiSample.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IPersistToDisc _persistToDisc;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IPersistToDisc persistToDisc)
    {
        _logger = logger;
        _persistToDisc = persistToDisc;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("Wettervorhersage wird erstellt.");
        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        _persistToDisc.SaveData(result, "C:\\temp");

        return result;
    }

    [HttpPost(Name = "PostWeatherForecast")]
    public void Post([FromBody] WeatherForecast forecast)
    {
        _logger.LogInformation("Wettervorhersage wird gespeichert.");
        _persistToDisc.SaveData(forecast, "C:\\temp");
    }
}
