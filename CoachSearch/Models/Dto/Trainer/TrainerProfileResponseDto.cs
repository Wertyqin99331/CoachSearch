using CoachSearch.Models.Dto.Review;
using CoachSearch.Models.Dto.TrainerProgram;

namespace CoachSearch.Models.Dto.ProfileDto;

public class TrainerProfileResponseDto
{
	public required string? Email { get; set; }

	public required string? PhoneNumber { get; set; }

	public required string FirstName { get; set; } = null!;

	public required string MiddleName { get; set; } = null!;

	public required string LastName { get; set; } = null!;
	
	public string City { get; set; } = null!;

	public required string Specialization { get; set; } = null!;
	
	public required string? Info { get; set; } 
	
	public required List<TrainingProgramDto>? TrainingPrograms { get; set; }
	
	public required string? AvatarUrl { get; set; }

	public required string? TelegramLink { get; set; }

	public required string? InstagramLink { get; set; }
}