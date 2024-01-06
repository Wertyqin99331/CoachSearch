using CoachSearch.Models.Enums;

namespace CoachSearch.Models.Dto;

public class LoginResponseDto
{
	public required string Token { get; set; } = null!;
	public required string Role { get; set; }
}