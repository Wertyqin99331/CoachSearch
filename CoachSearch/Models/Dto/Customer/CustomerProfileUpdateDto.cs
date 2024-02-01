using System.ComponentModel.DataAnnotations;
using CoachSearch.Models.Dto.TrainerProgram;
using CoachSearch.Services.FileUploadService;

namespace CoachSearch.Models.Dto.Customer;

public class CustomerProfileUpdateDto
{
	[Required] [MaxLength(100)] public string FullName { get; set; } = null!;

	[MaxLength(100)] public string? VkLink { get; set; }

	[MaxLength(100)] public string? TelegramLink { get; set; }

	[MaxLength(500)] public string? Info { get; set; }

	public IFormFile? Avatar { get; set; }


	public async Task<Data.Entities.Customer> ToCustomer(Data.Entities.Customer customer, IFileUploadService fileUploadService)
	{
		if (this.Avatar == null && customer.AvatarFileName != null)
			fileUploadService.DeleteFile(customer.AvatarFileName);
		
		return new Data.Entities.Customer()
		{
			CustomerId = customer.CustomerId,
			FullName = this.FullName,
			Info = this.Info,
			VkLink = this.VkLink,
			TelegramLink = this.TelegramLink,
			AvatarFileName = this.Avatar == null
				? null
				: await fileUploadService.UploadFileAsync(this.Avatar),
			UserInfoId = customer.UserInfoId,
			Likes = customer.Likes,
			Reviews = customer.Reviews
		};
	}
	
	
}