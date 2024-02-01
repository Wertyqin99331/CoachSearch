using System.Security.Claims;
using CoachSearch.Models.Enums;

namespace CoachSearch.Services.UserService;

public class UserService: IUserService
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public UserService(IHttpContextAccessor httpContextAccessor)
	{
		this._httpContextAccessor = httpContextAccessor;
	}

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
		if (this._httpContextAccessor.HttpContext is null) return null;
		var roleInString = this._httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
		if (Enum.TryParse(roleInString, true, out UserRole parseResult))
			return parseResult;

		return null;
	}

	private string? GetClaimValue(string claimType)
	{
		string? result = null;

		if (this._httpContextAccessor.HttpContext is not null)
			result = this._httpContextAccessor.HttpContext.User.FindFirstValue(claimType);

		return result == string.Empty ? null : result;
	}
}