using Core.AnchorCalculator.Entities;
using Microsoft.AspNetCore.Identity;

namespace UI.AnchorCalculator.Extensions;

public static class CurrentUser
{
	public static async Task<User> Get(UserManager<User> UserManager, string nameUser) => await UserManager.FindByNameAsync(nameUser);
}
