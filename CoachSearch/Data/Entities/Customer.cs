using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoachSearch.Models.Dto.ProfileDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CoachSearch.Data.Entities;

public class Customer
{
	public long CustomerId { get; set; }

	[MaxLength(25)]
	public string FirstName { get; set; } = null!;

	[MaxLength(25)]
	public string MiddleName { get; set; } = null!;

	[MaxLength(25)]
	public string LastName { get; set; } = null!;

	[MaxLength(25)]
	public string City { get; set; } = null!;
	
	[MaxLength(500)]
	public string? Info { get; set; }
	
	
	public long UserInfoId { get; set; }
	[ForeignKey("UserInfoId")] public virtual ApplicationUser UserInfo { get; set; } = null!;

	public virtual List<Review> Reviews { get; set; } = null!;
}