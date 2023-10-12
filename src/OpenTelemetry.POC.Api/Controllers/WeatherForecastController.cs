using System.Globalization;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.POC.Api.Database;

namespace OpenTelemetry.POC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
	private readonly ILogger<WeatherForecastController> _logger;
	private readonly AppDbContext _context;
	private readonly IBus _bus;

	public WeatherForecastController(IBus bus, AppDbContext context, ILogger<WeatherForecastController> logger)
		=> (_bus, _context, _logger) = (bus, context, logger);
	
	[HttpGet(Name = "GetWeatherForecast")]
	public async Task<IEnumerable<WeatherForecast>> Get()
	{
		// interaction with db using EF
		_context.TesModels.Add(new TestModel
		{
			Id = Guid.NewGuid(),
			Data = DateTime.Now.ToString(CultureInfo.InvariantCulture)
		});
		
		await _context.SaveChangesAsync();
		
		// interaction with sns/sqs using MassTransit
		await _bus.Publish<ITestMessage>(new
		{
			Data = "some test message"
		});
		
		_logger.LogInformation("Log from OpenTelemetry.POC.Api");


		return GetForecast();
	}

	private static IEnumerable<WeatherForecast> GetForecast()
		=> Enumerable.Range(1, 5)
			.Select(index => new WeatherForecast
			{
				Date = DateTime.Now.AddDays(index),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = WeatherForecast.Summaries[Random.Shared.Next(WeatherForecast.Summaries.Length)]
			})
			.ToArray();
}