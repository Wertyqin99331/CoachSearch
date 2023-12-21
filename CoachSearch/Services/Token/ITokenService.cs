using CoachSearch.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace CoachSearch.Services.Token;

public interface ITokenService
{
	string CreateToken(ApplicationUser user, IEnumerable<string> roles);
}