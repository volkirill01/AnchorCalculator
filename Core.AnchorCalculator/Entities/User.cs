using Microsoft.AspNetCore.Identity;

namespace Core.AnchorCalculator.Entities;

public class User : IdentityUser
{
	public HashSet<Anchor>? Anchors { get; private set; }

	public User()
	{
		Anchors = new();
	}
}
