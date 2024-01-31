using CoachSearch.Data.Entities;
using CoachSearch.Services.FileUploadService;

namespace CoachSearch.Models.Dto.Trainer;

public class AllTrainerDto
{
	public required long TrainerId { get; set; }

	public required string FullName { get; set; } = null!;

	public required string Specialization { get; set; } = null!;

	public required int LikesCount { get; set; }

	public required bool IsLiked { get; set; }

	public required string Address { get; set; }

	public required string? AvatarUrl { get; set; }
}