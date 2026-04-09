using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace UI.AnchorCalculator.ViewModels;

public class RegisterViewModel
{
	[Required(ErrorMessage = "Укажите логин")]
	[Display(Name = "Логин")]
	[Remote(action: "CheckExistAccountByUserName", controller: "Validation", ErrorMessage = "Такой пользователь уже существует")]
	public string UserName { get; set; }

	[Required(ErrorMessage = "Укажите email")]
	[Display(Name = "Email")]
	[Remote(action: "CheckExistAccountByEmail", controller: "Validation", ErrorMessage = "Такой пользователь уже существует")]
	public string Email { get; set; }

	[Required(ErrorMessage = "Введите пароль")]
	[DataType(DataType.Password)]
	[Display(Name = "Пароль")]
	public string Password { get; set; }

	[Required(ErrorMessage = "Повторите пароль")]
	[Compare("Password", ErrorMessage = "Пароли не совпадают")]
	[DataType(DataType.Password)]
	[Display(Name = "Подтвердить пароль")]
	public string PasswordConfirm { get; set; }
}
