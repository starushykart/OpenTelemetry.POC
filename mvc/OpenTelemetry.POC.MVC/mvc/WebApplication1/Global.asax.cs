using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WebApplication1
{
	public class MvcApplication : System.Web.HttpApplication
	{
		public static ActivitySource Source = new ActivitySource("Sample.DistributedTracing", "1.0.0");

		public static ICollection<Activity> ExportedItems = new List<Activity>();

		private TracerProvider _tracerProvider;

		protected void Application_Start()
		{
			_tracerProvider = Sdk.CreateTracerProviderBuilder()
				.AddSource("Sample.DistributedTracing")
				// .SetResourceBuilder(ResourceBuilder.CreateDefault()
				// 	.AddService("Sample.DistributedTracing")
				// 	.AddTelemetrySdk()
				// 	.AddEnvironmentVariableDetector())
				.AddAspNetInstrumentation()
				.AddHttpClientInstrumentation()
				.AddConsoleExporter()
				.AddOtlpExporter()
				.AddJaegerExporter(x => x.Protocol = JaegerExportProtocol.HttpBinaryThrift)
				.Build();
            
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}
		        
		protected void Application_End()
		{
			_tracerProvider?.Dispose();
		}
	}
}