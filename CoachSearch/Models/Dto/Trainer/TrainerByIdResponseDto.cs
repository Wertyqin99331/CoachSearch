using CoachSearch.Data.Entities;
using CoachSearch.Models.Dto.Review;
using CoachSearch.Models.Dto.TrainerProgram;

namespace CoachSearch.Models.Dto;

public class TrainerByIdResponseDto
{
	public required long TrainerId { get; set; }
	
	public required string FullName { get; set; }
	
	public required string Address { get; set; }
	
	public required string? Email { get; set; }
	
	public required string? PhoneNumber { get; set; }
	
	public required string? Info { get; set; }

	public required string Specialization { get; set; } = null!;

	public required int LikesCount { get; set; }
	
	public required bool IsLiked { get; set; }

	public required string? AvatarUrl { get; set; }

	public required string? TelegramLink { get; set; } 

	public required string? VkLink { get; set; }
	
	public required List<TrainingProgramDto> TrainingPrograms { get; set; }
	
	public required List<ReviewDto> Reviews { get; set; }
}