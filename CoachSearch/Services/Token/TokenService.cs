using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoachSearch.Data.Entities;
using Microsoft.IdentityModel.Tokens;

namespace CoachSearch.Services.Token;

public class TokenService: ITokenService
{
	private readonly IConfiguration _configuration;

	public TokenService(IConfiguration configuration)
	{
		this._configuration = configuration;
	}

	public string CreateToken(ApplicationUser user, IEnumerable<string> roles)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.Name, user.UserName!),
			new(ClaimTypes.Email, user.Email ?? string.Empty),
			new(ClaimTypes.Role, string.Join(" ", roles)),
			new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
		};

		var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._configuration["Jwt:Secret"]!));
		var signInCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256);

		var jwt = new JwtSecurityToken(
			issuer: this._configuration["Jwt:Issuer"],
			audience: this._configuration["Jwt:Audience"],
			claims: claims,
			expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
			notBefore: DateTime.Now,
			signingCredentials: signInCredentials);

		return new JwtSecurityTokenHandler().WriteToken(jwt);
	}
}