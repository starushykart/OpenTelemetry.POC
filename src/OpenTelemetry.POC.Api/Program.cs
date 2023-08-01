using System.Diagnostics;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using MassTransit.Logging;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.POC.Api.Database;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(x => x
	.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddMassTransit(cfg =>
{
	cfg.UsingAmazonSqs((context, x) =>
	{
		var url = new Uri(builder.Configuration.GetValue<string>("LocalstackUrl"));
        
		x.Host(new Uri($"amazonsqs://{url.Authority}"), h =>
		{
			h.AccessKey("admin");
			h.SecretKey("admin");
            
			h.Config(new AmazonSQSConfig { ServiceURL = url.ToString() });
			h.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = url.ToString() });
		});
		
		x.ConfigureEndpoints(context);
	});
});

builder.Services
	.AddOpenTelemetry()
	.ConfigureResource(x => x.AddService(builder.Environment.ApplicationName))
	.WithTracing(x => x
		.AddSource(DiagnosticHeaders.DefaultListenerName)
		.AddAspNetCoreInstrumentation()
		//.AddEntityFrameworkCoreInstrumentation()
		.AddNpgsql()
		//.AddHttpClientInstrumentation()
		.AddConsoleExporter()
		.AddJaegerExporter());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("trace-id", Activity.Current?.TraceId.ToString());
    await next();
});

app.MapControllers();

app.Run();

namespace OpenTelemetry.POC.Api
{
	public static class DiagnosticsConfig
	{
		public const string ServiceName = "MyService";
		public static ActivitySource ActivitySource = new(ServiceName);
	}
}