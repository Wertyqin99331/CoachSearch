using Microsoft.AspNetCore.Identity;

namespace CoachSearch.Data.Entities;

public class ApplicationUser: IdentityUser<long>
{
	public virtual Customer? Customer { get; set; }
	public virtual Trainer? Trainer { get; set; }
}