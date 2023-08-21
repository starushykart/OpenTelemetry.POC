using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.POC.Web.Extensions;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication
    .CreateBuilder(args)
    .UseSerilog();

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(x => x
        .AddService(builder.Environment.ApplicationName))
    .WithTracing(x => x
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter(o =>
        {
            o.ExportProcessorType = ExportProcessorType.Simple;
            o.Protocol = OtlpExportProtocol.HttpProtobuf;
        })
        .AddZipkinExporter());

builder.Services.AddHttpClient("WeatherClient", x =>
{
    x.BaseAddress = builder.Configuration.GetValue<Uri>("WeatherClient:BaseUrl");
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");;

app.Run();

