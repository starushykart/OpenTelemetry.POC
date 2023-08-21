using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using MassTransit.Logging;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.POC.Api.Database;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication
	.CreateBuilder(args);

builder.Services
	.AddOpenTelemetry()
	.ConfigureResource(x => x.AddService(builder.Environment.ApplicationName))
	.WithTracing(x => x
		.AddSource(DiagnosticHeaders.DefaultListenerName)
		.AddAspNetCoreInstrumentation()
		.AddHttpClientInstrumentation()
		.AddNpgsql()
		.AddConsoleExporter()
		.AddJaegerExporter()
		.AddZipkinExporter());

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

app.MapControllers();

app.Run();