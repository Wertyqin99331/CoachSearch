using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoachSearch.Models.Dto.ProfileDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CoachSearch.Data.Entities;

public class Customer
{
	public long CustomerId { get; set; }

	[MaxLength(100)]
	public required string FullName { get; set; } = null!;
	
	[MaxLength(500)]
	public required string? Info { get; set; }
	
	[MaxLength(100)]
	public required string? VkLink { get; set; }
	
	[MaxLength(100)]
	public required string? TelegramLink { get; set; }
	
	[MaxLength(1000)]
	public required string? AvatarFileName { get; set; }
	
	public long UserInfoId { get; set; }
	[ForeignKey("UserInfoId")] public virtual ApplicationUser UserInfo { get; set; } = null!;

	public virtual List<Review> Reviews { get; set; } = null!;

	public virtual List<Like> Likes { get; set; } = null!;
}