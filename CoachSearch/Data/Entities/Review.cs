using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachSearch.Data.Entities;

public class Review
{
	public long ReviewId { get; set; }

	[MaxLength(50)] public string ReviewTitle { get; set; } = null!;

	[MaxLength(500)] public string ReviewText { get; set; } = null!;

	public DateOnly ReviewDate { get; set; }

	public long CustomerId { get; set; }
	[ForeignKey(nameof(CustomerId))] public virtual Customer Customer { get; set; } = null!;

	public long TrainerId { get; set; }
	[ForeignKey(nameof(TrainerId))] public virtual Trainer Trainer { get; set; } = null!;
}