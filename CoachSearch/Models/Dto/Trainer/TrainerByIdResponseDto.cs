using CoachSearch.Data.Entities;
using CoachSearch.Models.Dto.Review;
using CoachSearch.Models.Dto.TrainerProgram;

namespace CoachSearch.Models.Dto;

public class TrainerByIdResponseDto
{
	public required long TrainerId { get; set; }
	
	public required string FirstName { get; set; } = null!;

	public required string MiddleName { get; set; } = null!;

	public required string LastName { get; set; } = null!;

	public string City { get; set; } = null!;

	public required string? Info { get; set; }

	public required string Specialization { get; set; } = null!;

	public required string? AvatarUrl { get; set; }

	public required string? TelegramLink { get; set; } 

	public required string? InstagramLink { get; set; }
	
	public required List<TrainingProgramDto> TrainingPrograms { get; set; }
	
	public required List<ReviewDto> Reviews { get; set; }
}