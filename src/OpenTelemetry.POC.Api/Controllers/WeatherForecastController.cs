using System.Globalization;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.POC.Api.Database;
using OpenTelemetry.POC.Api.Models;

namespace OpenTelemetry.POC.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
	[HttpGet(Name = "GetWeatherForecast")]
	public async Task<IEnumerable<WeatherForecast>> Get(
		[FromServices] IBus bus,
		[FromServices] AppDbContext context)
	{
		// interaction with db using EF
		context.TesModels.Add(new TestModel
		{
			Id = Guid.NewGuid(),
			Data = DateTime.Now.ToString(CultureInfo.InvariantCulture)
		});
		await context.SaveChangesAsync();
		
		// interaction with sns/sqs using MassTransit
		await bus.Publish<ITestMessage>(new
		{
			Data = "some test message"
		});

		return Enumerable.Range(1, 5)
			.Select(index => new WeatherForecast
			{
				Date = DateTime.Now.AddDays(index),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = WeatherForecast.Summaries[Random.Shared.Next(WeatherForecast.Summaries.Length)]
			})
			.ToArray();
	}
}