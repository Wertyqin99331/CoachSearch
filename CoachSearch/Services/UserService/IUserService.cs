using CoachSearch.Models.Enums;

namespace CoachSearch.Services.UserService;

public interface IUserService
{
	string? GetMyEmail();
	string? GetMyPhoneNumber();

	(string? email, string? phoneNumber) GetCredentials();

	UserRole? GetUserRole();
}