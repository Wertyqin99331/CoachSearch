using System.ComponentModel.DataAnnotations;
using CoachSearch.Models.Enums;

namespace CoachSearch.Models.Dto.Review;

public class TrainerRegistrationRequestDto
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
	[MaxLength(100)]
	public string FullName { get; set; } = null!;

	[Required] public string Specialization { get; set; } = null!;
	
	[MaxLength(500)]
	public string? Info { get; set; }
	
	[MaxLength(100)]
	public string? VkLink { get; set; }
	
	[MaxLength(100)]
	public string? TelegramLink { get; set; }
	
	
}