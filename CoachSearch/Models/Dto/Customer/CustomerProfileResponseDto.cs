namespace CoachSearch.Models.Dto.ProfileDto;

public class CustomerProfileResponseDto
{
	public required long CustomerId { get; set; }
	
	public required string? Email { get; set; }

	public required string? PhoneNumber { get; set; }
	
	public required string FullName { get; set; }
	
	public required string? VkLink { get; set; }
	
	public required string? TelegramLink { get; set; }
	
	public required string? Info { get; set; }
}