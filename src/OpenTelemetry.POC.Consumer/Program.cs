using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using MassTransit.Logging;
using OpenTelemetry.POC.Consumer.Consumers;
using OpenTelemetry.POC.Consumer.Extensions;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication
	.CreateBuilder(args)
	.UseSerilog();

builder.Services.AddOpenTelemetry()
	.ConfigureResource(x => x.AddService(builder.Environment.ApplicationName))
	.WithTracing(x => x
		.AddSource(DiagnosticHeaders.DefaultListenerName)
		.AddAspNetCoreInstrumentation()
		.AddHttpClientInstrumentation()
		.AddZipkinExporter());

builder.Services.AddMassTransit(cfg =>
{
	cfg.AddConsumers(typeof(TestMessageConsumer).Assembly);

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

var app = builder.Build();

app.Run();