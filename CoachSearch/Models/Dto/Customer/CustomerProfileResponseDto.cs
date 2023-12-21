namespace CoachSearch.Models.Dto.ProfileDto;

public class CustomerProfileResponseDto
{
	public required long CustomerId { get; set; }
	
	public required string? Email { get; set; }

	public required string? PhoneNumber { get; set; }
	
	public required string FirstName { get; set; }

	public required string MiddleName { get; set; }

	public required string LastName { get; set; }

	public required string City { get; set; } 
	
	public required string? Info { get; set; }
}