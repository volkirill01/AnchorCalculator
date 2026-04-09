using DAL.AnchorCalculator;
using Microsoft.AspNetCore.Mvc;

namespace UI.AnchorCalculator.Controllers;

public class ValidationController : Controller
{
	private readonly ApplicationDbContext m_ApplicationDbContext;

	public ValidationController(ApplicationDbContext applicationDbContext)
	{
		m_ApplicationDbContext = applicationDbContext;
	}

	[AcceptVerbs("GET", "POST")]
	public bool CheckExistAccountByEmail(string Email) => !m_ApplicationDbContext.Users.Any(e => e.Email == Email);

	[AcceptVerbs("GET", "POST")]
	public bool CheckExistAccountByUserName(string UserName) => !m_ApplicationDbContext.Users.Any(e => e.UserName == UserName);
}
