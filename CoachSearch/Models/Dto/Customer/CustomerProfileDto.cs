using CoachSearch.Data.Entities;
using CoachSearch.Services.FileUploadService;

namespace CoachSearch.Models.Dto.Customer;

public class CustomerProfileDto
{
	public required long CustomerId { get; set; }
	
	public required string? Email { get; set; }

	public required string? PhoneNumber { get; set; }
	
	public required string FullName { get; set; }
	
	public required string? VkLink { get; set; }
	
	public required string? TelegramLink { get; set; }
	
	public required string? Info { get; set; }
	
	public required string? AvatarUrl { get; set; }
	
	
	public static CustomerProfileDto FromCustomer(Data.Entities.Customer customer, ApplicationUser user, IFileUploadService fileUploadService, HttpRequest request)
	{
		return new CustomerProfileDto()
		{
			CustomerId = customer.CustomerId,
			Email = user.Email,
			PhoneNumber = user.PhoneNumber,
			FullName = customer.FullName,
			Info = customer.Info,
			TelegramLink = customer.TelegramLink,
			VkLink = customer.VkLink,
			AvatarUrl = fileUploadService.GetAvatarUrl(request, customer.AvatarFileName)
		};
	}
}