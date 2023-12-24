using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoachSearch.Models.Dto;
using Microsoft.Extensions.Primitives;

namespace CoachSearch.Data.Entities;

public class Trainer
{
	public long TrainerId { get; set; }

	[MaxLength(25)] public string FirstName { get; set; } = null!;

	[MaxLength(25)] public string MiddleName { get; set; } = null!;

	[MaxLength(25)] public string LastName { get; set; } = null!;

	[MaxLength(25)] public string City { get; set; } = null!;

	[MaxLength(300)] public string Specialization { get; set; } = null!;

	[MaxLength(500)] public string? Info { get; set; }

	[MaxLength(1000)] public string? AvatarFileName { get; set; }

	[MaxLength(100)] public string? TelegramLink { get; set; }

	[MaxLength(100)] public string? InstagramLink { get; set; }

	
	[MaxLength(50)] public long UserInfoId { get; set; }
	[ForeignKey("UserInfoId")] public virtual ApplicationUser UserInfo { get; set; } = null!;
	
	public virtual List<TrainingProgram> TrainingPrograms { get; set; } = null!;
	public virtual List<Review> Reviews { get; set; } = null!;

	public virtual List<Like> Likes { get; set; } = null!;
}