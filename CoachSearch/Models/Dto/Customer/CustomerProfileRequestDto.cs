using System.ComponentModel.DataAnnotations;
using CoachSearch.Models.Dto.TrainerProgram;

namespace CoachSearch.Models.Dto.ProfileDto;

public class CustomerProfileRequestDto
{
	[Required] [MaxLength(25)] public string FirstName { get; set; } = null!;

	[Required] [MaxLength(25)] public string MiddleName { get; set; } = null!;

	[Required] [MaxLength(25)] public string LastName { get; set; } = null!;

	[Required] [MaxLength(25)] public string City { get; set; } = null!;
	
	[MaxLength(500)] public string? Info { get; set; }
}