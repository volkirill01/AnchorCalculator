using Core.AnchorCalculator.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.AnchorCalculator.Extensions;
using UI.AnchorCalculator.ViewModels;

namespace UI.AnchorCalculator.Controllers;

public class AccountController : Controller
{
	private readonly UserManager<User> m_UserManager;
	private readonly SignInManager<User> m_SignInManager;

	public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
	{
		m_UserManager = userManager;
		m_SignInManager = signInManager;
	}

	[HttpGet]
	public IActionResult Register() => View();

	[HttpPost]
	public async Task<IActionResult> Register(RegisterViewModel model)
	{
		if (ModelState.IsValid)
		{
			User user = new()
			{
				Email = model.Email,
				UserName = model.UserName
			};

			var result = await m_UserManager.CreateAsync(user, model.Password);
			if (result.Succeeded)
			{
				await m_UserManager.AddToRoleAsync(user, "employee");
				await m_SignInManager.SignInAsync(user, false);

				return RedirectToAction("Index", "Home");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}
		return View(model);
	}

	[HttpGet]
	public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel{ ReturnUrl = returnUrl });

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(LoginViewModel model)
	{
		if (ModelState.IsValid)
		{
			User? user = await m_UserManager.FindByNameAsync(model.EmailLogin) ?? await m_UserManager.FindByEmailAsync(model.EmailLogin);
			if (user != null)
			{
				var result = await m_SignInManager.PasswordSignInAsync(
					user,
					model.Password,
					model.RememberMe,
					false
				);

				if (result.Succeeded)
				{
					if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
						return Redirect(model.ReturnUrl);

					return RedirectToAction("Index", "Anchor");
				}
				ModelState.AddModelError("", "Неправильный логин, электронная почта и (или) пароль");
			}
		}
		return View(model);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> LogOut()
	{
		await m_SignInManager.SignOutAsync();

		return RedirectToAction("Login", "Account");
	}

	[HttpPost]
	public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
	{
		if (ModelState.IsValid)
		{
			User? user = await m_UserManager.FindByIdAsync(model.Id);
			if (user != null)
			{
				IdentityResult result = await m_UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

				if (result.Succeeded)
					return RedirectToAction("PrivateCabinet");

				foreach (var error in result.Errors)
				{
					string message = error.Description;
					if (message == "Incorrect password.")
						message = "Неверный пароль.";

					ModelState.AddModelError(string.Empty, message);
				}
			}
			else
				ModelState.AddModelError(string.Empty, "Пользователь не найден");
		}
		return View(model);
	}

	[HttpGet]
	public async Task<IActionResult> ChangePassword(string id)
	{
		User? user = await m_UserManager.FindByIdAsync(id);
		if (user == null)
			return NotFound();

		ChangePasswordViewModel model = new(){ Id = user.Id, Email = user.Email, UserName = user.UserName };
		return View(model);
	}

	[HttpGet]
	public async Task<IActionResult> PrivateCabinet()
	{
		User? userCur = await CurrentUser.Get(m_UserManager, User.Identity.Name);
		if (userCur == null)
			return NotFound();

		return View(userCur);
	}
}
