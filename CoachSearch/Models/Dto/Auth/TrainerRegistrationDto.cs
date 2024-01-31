using System.ComponentModel.DataAnnotations;
using CoachSearch.Data.Entities;
using CoachSearch.Models.Enums;
using CoachSearch.Services.FileUploadService;

namespace CoachSearch.Models.Dto.Auth;

public class TrainerRegistrationDto
{
	[EmailAddress] 
	public string? Email { get; set; } = null!;
	
	[MinLength(11)]
	[MaxLength(11)]
	public string? PhoneNumber { get; set; }

	[Required]
	[MinLength(6)]
	[MaxLength(30)]
	public string Password { get; set; } = null!;

	[Required]
	[MaxLength(100)]
	public string FullName { get; set; } = null!;

	[Required] [MaxLength(150)] public string Address { get; set; } = null!;

	[Required] public string Specialization { get; set; } = null!;
	
	[MaxLength(500)]
	public string? Info { get; set; }
	
	[MaxLength(100)]
	public string? VkLink { get; set; }
	
	[MaxLength(100)]
	public string? TelegramLink { get; set; }
	
	public IFormFile? Avatar { get; set; }


	public async Task<Data.Entities.Trainer> ToTrainer(ApplicationUser user, IFileUploadService fileUploadService)
	{
		return new Data.Entities.Trainer()
		{
			FullName = this.FullName,
			Info = this.Info,
			Address = this.Address,
			TelegramLink = this.TelegramLink,
			VkLink = this.VkLink,
			UserInfo = user,
			Specialization = this.Specialization,
			AvatarFileName = this.Avatar == null
				? null
				: await fileUploadService.UploadFileAsync(this.Avatar)
		};
	}
}