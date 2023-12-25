using System.ComponentModel.DataAnnotations;

namespace CoachSearch.Models.Dto.TrainerProgram;

public class TrainingProgramRequestDto
{
	[Required]
	public required string TrainingProgramName { get; set; } = null!;

	[Required]
	public required int TrainingProgramPrice { get; set; }
}