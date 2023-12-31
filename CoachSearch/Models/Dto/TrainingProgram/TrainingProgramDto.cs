﻿using System.ComponentModel.DataAnnotations;

namespace CoachSearch.Models.Dto.TrainerProgram;

public class TrainingProgramDto
{
	public required long TrainingProgramId { get; set; }
	
	[Required]
	public required string TrainingProgramName { get; set; } = null!;

	[Required]
	public required int TrainingProgramPrice { get; set; }
}