using System.ComponentModel.DataAnnotations;

namespace CoachSearch.Models.Dto.Auth;

public class LoginRequestDto
{
	[Required] [MaxLength(50)] public required string Login { get; set; } = null!;

	[Required]
	[MinLength(6)]
	[MaxLength(30)]
	public required string Password { get; set; } = null!;
}