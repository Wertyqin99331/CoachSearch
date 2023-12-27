using System.ComponentModel.DataAnnotations;
using CoachSearch.Models.Dto.TrainerProgram;

namespace CoachSearch.Models.Dto.ProfileDto;

public class CustomerProfileRequestDto
{
	[Required] [MaxLength(100)] public string FullName { get; set; } = null!;

	[MaxLength(100)] public string? VkLink { get; set; }

	[MaxLength(100)] public string? TelegramLink { get; set; }

	[MaxLength(500)] public string? Info { get; set; }
}