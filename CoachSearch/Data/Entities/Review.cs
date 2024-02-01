using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoachSearch.Data.Entities;

[EntityTypeConfiguration(typeof(ReviewEntityConfiguration))]
public class Review
{
	public long ReviewId { get; set; }


	[MaxLength(500)] public string ReviewText { get; set; } = null!;

	public DateTime ReviewDate { get; set; }

	public long CustomerId { get; set; }
	[ForeignKey(nameof(CustomerId))] public virtual Customer Customer { get; set; } = null!;

	public long TrainerId { get; set; }
	[ForeignKey(nameof(TrainerId))] public virtual Trainer Trainer { get; set; } = null!;
}

public class ReviewEntityConfiguration : IEntityTypeConfiguration<Review>
{
	public void Configure(EntityTypeBuilder<Review> builder)
	{
		builder
			.HasOne(r => r.Trainer)
			.WithMany(t => t.Reviews)
			.OnDelete(DeleteBehavior.Restrict);

		builder
			.HasOne(r => r.Customer)
			.WithMany(c => c.Reviews)
			.OnDelete(DeleteBehavior.Restrict);
	}
}