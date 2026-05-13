using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using UI.AnchorCalculator.Models;

namespace UI.AnchorCalculator.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		public readonly ILogger<HomeController> m_Logger;

		public HomeController(ILogger<HomeController> logger)
		{
			m_Logger = logger;
		}

		public IActionResult Index() => View();
		public IActionResult Privacy() => View();

		[AllowAnonymous]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() => View(new ErrorViewModel{ RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
