using System.ComponentModel.DataAnnotations;
using CoachSearch.Models.Dto.TrainerProgram;

namespace CoachSearch.Models.Dto.Trainer;

public class UpdateTrainingProgramsRequestDto
{
	[Required] public List<TrainingProgramRequestDto> TrainingPrograms { get; set; } = null!;
}