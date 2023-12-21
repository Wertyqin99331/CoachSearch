using System.ComponentModel.DataAnnotations;

namespace CoachSearch.Models.Dto.TrainerProgram;

public class TrainingProgramDto
{
	[Required]
	public string TrainingProgramName { get; set; } = null!;

	[Required]
	public int TrainingProgramPrice { get; set; }
}