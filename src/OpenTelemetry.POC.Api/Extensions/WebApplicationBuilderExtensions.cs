using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Compact;

namespace OpenTelemetry.POC.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
	public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder builder)
	{
		builder.Logging.ClearProviders();
		
		builder.Host.UseSerilog((_, configuration) =>
		{
			configuration
				.MinimumLevel.Information()
				.Enrich.WithSpan()
				.WriteTo.Console(new CompactJsonFormatter());
		});

		return builder;
	}
}