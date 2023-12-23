using System.ComponentModel.DataAnnotations;

namespace CoachSearch.Models.Dto.Like;

public class ToggleLikeRequestDto
{
	[Required]
	public long TrainerId { get; set; }
}