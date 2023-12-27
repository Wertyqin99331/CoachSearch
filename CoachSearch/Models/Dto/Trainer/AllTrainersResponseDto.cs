using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachSearch.Models.Dto;

public class AllTrainersResponseDto
{
	public required long TrainerId { get; set; }

	public required string FullName { get; set; } = null!;

	public required string Specialization { get; set; } = null!;
	
	public required int LikesCount { get; set; }
	
	public required string? AvatarUrl { get; set; }
}