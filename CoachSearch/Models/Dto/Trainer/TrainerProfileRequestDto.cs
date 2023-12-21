using System.ComponentModel.DataAnnotations;
using CoachSearch.Models.Dto.TrainerProgram;

namespace CoachSearch.Models.Dto;

public class TrainerProfileRequestDto
{
	[Required] [MaxLength(25)] public string FirstName { get; set; } = null!;

	[Required] [MaxLength(25)] public string MiddleName { get; set; } = null!;

	[Required] [MaxLength(25)] public string LastName { get; set; } = null!;

	[Required] [MaxLength(25)] public string City { get; set; } = null!;
	
	[Required] [MaxLength(300)] public string Specialization { get; set; } = null!;

	[Required] public List<TrainingProgramDto> TrainingPrograms { get; set; } = null!;
	
	[MaxLength(500)] public string? Info { get; set; }
	
	public string? TelegramLink { get; set; }
	
	public string? InstagramLink { get; set; }
}