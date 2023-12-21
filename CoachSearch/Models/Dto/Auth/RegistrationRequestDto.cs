using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CoachSearch.Models.Enums;

namespace CoachSearch.Models.Dto;

public class RegistrationRequestDto
{
	[EmailAddress] 
	public string? Email { get; set; } = null!;
	
	[MinLength(11)]
	[MaxLength(11)]
	public string? PhoneNumber { get; set; }

	[Required]
	[MinLength(6)]
	[MaxLength(30)]
	public string Password { get; set; } = null!;
	
	[Required]
	public UserRole UserRole { get; set; }
}