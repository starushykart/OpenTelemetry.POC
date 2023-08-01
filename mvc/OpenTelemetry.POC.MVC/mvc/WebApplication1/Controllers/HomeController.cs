using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			using (var activity = MvcApplication.Source.StartActivity("SomeWork"))
			{
				var client = new HttpClient();
				client.BaseAddress = new Uri("http://localhost:54203");
				var a = client.GetAsync("/weatherforecast").GetAwaiter().GetResult();
				var b = a.Content.ReadAsStringAsync().GetAwaiter().GetResult();
			}

			ViewBag.Message = "Your application description page.";
			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";
			return View();
		}
	}
}