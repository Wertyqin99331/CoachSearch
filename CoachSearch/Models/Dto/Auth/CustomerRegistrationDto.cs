using System.ComponentModel.DataAnnotations;
using CoachSearch.Data.Entities;
using CoachSearch.Models.Enums;
using CoachSearch.Services.FileUploadService;

namespace CoachSearch.Models.Dto;

public class CustomerRegistrationDto
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
	
	[MaxLength(500)]
	public string? Info { get; set; }
	
	[MaxLength(100)]
	public string? VkLink { get; set; }
	
	[MaxLength(100)]
	public string? TelegramLink { get; set; }
	
	public IFormFile? Avatar { get; set; }


	public async Task<Data.Entities.Customer> ToCustomer(ApplicationUser user, IFileUploadService fileUploadService)
	{
		return new Data.Entities.Customer()
		{
			FullName = this.FullName,
			Info = this.Info,
			TelegramLink = this.TelegramLink,
			VkLink = this.VkLink,
			UserInfo = user,
			AvatarFileName = this.Avatar == null
				? null
				: await fileUploadService.UploadFileAsync(this.Avatar)
		};
	}
}