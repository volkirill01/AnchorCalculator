using Core.AnchorCalculator.Entities;
using DAL.AnchorCalculator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.AnchorCalculator.ViewModels;

namespace UI.AnchorCalculator.Controllers;

public class UsersController : Controller
{
	private readonly UserManager<User> m_UserManager;
	private readonly ApplicationDbContext m_ApplicationDbContext;

	public UsersController(UserManager<User> userManager, ApplicationDbContext applicationDbContext)
	{
		m_UserManager = userManager;
		m_ApplicationDbContext = applicationDbContext;
	}

	[HttpGet]
	public IActionResult Index() => View(m_UserManager.Users.ToList().Where(e => e.UserName != User.Identity.Name));

	[HttpGet]
	public IActionResult Create() => View();

	[HttpPost]
	public async Task<IActionResult> Create(RegisterViewModel model)
	{
		if (ModelState.IsValid)
		{
			User user = new() { Email = model.Email, UserName = model.UserName};

			var result = await m_UserManager.CreateAsync(user, model.Password);
			if (result.Succeeded)
				return RedirectToAction("Index");
			else
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
		}
		return View(model);
	}

	[HttpPost]
	public async Task<ActionResult> Delete(string id)
	{
		User user = await m_UserManager.FindByIdAsync(id);
		if (user != null)
		{
			await m_UserManager.DeleteAsync(user);
			await m_ApplicationDbContext.SaveChangesAsync();
		}
		return Ok();
	}
}
