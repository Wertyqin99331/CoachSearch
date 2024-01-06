using System.ComponentModel.DataAnnotations;
using CoachSearch.Models.Enums;

namespace CoachSearch.Models.Dto;

public class LoginRequestDto
{
	/*[EmailAddress]
	public string? Email { get; set; }

	[MinLength(11)]
	[MaxLength(11)]
	public string? PhoneNumber { get; set; }*/

	[Required] [MaxLength(50)] public required string Login { get; set; } = null!;

	[Required]
	[MinLength(6)]
	[MaxLength(30)]
	public required string Password { get; set; } = null!;
}