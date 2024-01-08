using System.ComponentModel.DataAnnotations;
using CoachSearch.Models.Dto.TrainerProgram;

namespace CoachSearch.Models.Dto;

public class TrainerProfileRequestDto
{
	[Required] [MaxLength(100)] public string FullName { get; set; } = null!;
	
	[Required] [MaxLength(300)] public string Specialization { get; set; } = null!;

	[Required] [MaxLength(150)] public string Address { get; set; } = null!;

	/*[Required] public List<TrainingProgramRequestDto> TrainingPrograms { get; set; } = null!;*/
	
	[MaxLength(500)] public string? Info { get; set; }
	
	public string? TelegramLink { get; set; }
	
	public string? VkLink { get; set; }
	
	public IFormFile? Avatar { get; set; }
}