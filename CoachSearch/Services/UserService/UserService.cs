using System.Security.Claims;
using CoachSearch.Models.Enums;

namespace CoachSearch.Services.UserService;

public class UserService(IHttpContextAccessor httpContextAccessor): IUserService
{
	public string? GetMyEmail()
	{
		return GetClaimValue(ClaimTypes.Email);
	}

	public string? GetMyPhoneNumber()
	{
		return GetClaimValue(ClaimTypes.MobilePhone);
	}

	public (string? email, string? phoneNumber) GetCredentials()
	{
		return (GetMyEmail(), GetMyPhoneNumber());
	}

	public UserRole? GetUserRole()
	{
		if (httpContextAccessor.HttpContext is null) return null;
		var roleInString = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
		if (Enum.TryParse(roleInString, true, out UserRole parseResult))
			return parseResult;

		return null;
	}

	private string? GetClaimValue(string claimType)
	{
		string? result = null;

		if (httpContextAccessor.HttpContext is not null)
			result = httpContextAccessor.HttpContext.User.FindFirstValue(claimType);

		return result == string.Empty ? null : result;
	}
}