using CoachSearch.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoachSearch.Extensions;

public static class UserManagerExtensions
{
	public static Task<ApplicationUser?>? FindByPhoneNumberAsync(this UserManager<ApplicationUser> userManager,
		string phoneNumber)
	{
		return userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
	}

	public static Task<ApplicationUser?> FindByIdAsync(this UserManager<ApplicationUser> userManager, long id)
	{
		return userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
	}

	public static Task<ApplicationUser?> FindByCredentialsAsync(this UserManager<ApplicationUser> userManager, string? email,
		string? phoneNumber)
	{
		if (email != null)
			return userManager.FindByEmailAsync(email);
		
		if (phoneNumber != null)
			return userManager.FindByPhoneNumberAsync(phoneNumber);

		return null;
	}

}