using System.ComponentModel.DataAnnotations;

namespace UI.AnchorCalculator.ViewModels;

public class LoginViewModel
{
	[Required(ErrorMessage = "Введите email или логин")]
	[Display(Name = "Email или Логин")]
	public string EmailLogin { get; set; }

	[Required(ErrorMessage = "Введите пароль")]
	[DataType(DataType.Password)]
	[Display(Name = "Пароль")]
	public string Password { get; set; }

	[Display(Name = "Запомнить")]
	public bool RememberMe { get; set; }

	public string? ReturnUrl { get; set; }
}
