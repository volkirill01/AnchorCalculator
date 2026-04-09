using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UI.AnchorCalculator.Models;

namespace UI.AnchorCalculator.Controllers
{
	public class HomeController : Controller
	{
		public readonly ILogger<HomeController> m_Logger;

		public HomeController(ILogger<HomeController> logger)
		{
			m_Logger = logger;
		}

		public IActionResult Index() => View();
		public IActionResult Privacy() => View();

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() => View(new ErrorViewModel{ RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}