using System.ComponentModel.DataAnnotations;

namespace CoachSearch.Models.Dto;

public class ChangePasswordRequestDto
{
	[Required]
	public string CurrentPassword { get; set; } = null!;

	[Required] public string NewPassword { get; set; } = null!;
}