using System.ComponentModel.DataAnnotations;
using CoachSearch.Services.FileUploadService;

namespace CoachSearch.Models.Dto.Trainer;

public class TrainerProfileUpdateDto
{
	[Required] [MaxLength(100)] public string FullName { get; set; } = null!;
	
	[Required] [MaxLength(300)] public string Specialization { get; set; } = null!;

	[Required] [MaxLength(150)] public string Address { get; set; } = null!;

	/*[Required] public List<TrainingProgramRequestDto> TrainingPrograms { get; set; } = null!;*/
	
	[MaxLength(500)] public string? Info { get; set; }
	
	public string? TelegramLink { get; set; }
	
	public string? VkLink { get; set; }
	
	public IFormFile? Avatar { get; set; }
	
	
	public async Task<Data.Entities.Trainer> ToTrainer(Data.Entities.Trainer trainer, IFileUploadService fileUploadService)
	{
		if (this.Avatar == null && trainer.AvatarFileName != null)
			fileUploadService.DeleteFile(trainer.AvatarFileName);
		
		return new Data.Entities.Trainer()
		{
			TrainerId = trainer.TrainerId,
			FullName = this.FullName,
			Info = this.Info,
			VkLink = this.VkLink,
			TelegramLink = this.TelegramLink,
			Address = this.Address,
			Specialization = this.Specialization,
			AvatarFileName = this.Avatar == null
				? null
				: await fileUploadService.UploadFileAsync(this.Avatar),
			UserInfoId = trainer.UserInfoId,
			Likes = trainer.Likes,
			Reviews = trainer.Reviews,
		};
	}
}