using System.ComponentModel.DataAnnotations;

namespace CoachSearch.Models.Dto.Review;

public class AddReviewRequestDto
{
	[Required] public long TrainerId { get; set; }
	
	[Required] [MaxLength(50)] public string ReviewTitle { get; set; } = null!;

	[Required] [MaxLength(500)] public string ReviewText { get; set; } = null!;
}