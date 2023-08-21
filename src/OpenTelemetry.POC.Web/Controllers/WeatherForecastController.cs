using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace OpenTelemetry.POC.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly HttpClient _client;

    public WeatherForecastController(IHttpClientFactory clientFactory ,ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
        _client = clientFactory.CreateClient("WeatherClient");
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        _logger.LogError("test");
        var result = await _client.GetFromJsonAsync<IEnumerable<WeatherForecast>>("api/WeatherForecast");
        
        return result;
    }
}

