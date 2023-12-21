using System.ComponentModel.DataAnnotations;

namespace CoachSearch.Models.Dto;

public class LoginRequestDto
{
	[EmailAddress]
	public string? Email { get; set; }
	
	[MinLength(11)]
	[MaxLength(11)]
	public string? PhoneNumber { get; set; }

	[Required]
	[MinLength(6)]
	[MaxLength(30)]
	public string Password { get; set; } = null!;
}