using System.ComponentModel.DataAnnotations;

namespace CoachSearch.Models.Dto.Review;

public class ReviewDto
{
	[Required] public required DateOnly ReviewDate { get; set; }

	[Required] public required string CustomerName { get; set; } = null!;

	[Required] public required string ProgramName { get; set; } = null!;

	[Required] public required string ReviewText { get; set; } = null!;
}