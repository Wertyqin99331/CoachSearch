using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoachSearch.Data.Entities;

[EntityTypeConfiguration(typeof(LikeEntityConfiguration))]
public class Like
{
	public long CustomerId { get; set; }
	[ForeignKey(nameof(CustomerId))]
	public virtual Customer Customer { get; set; } = null!;
	
	public long TrainerId { get; set; }
	[ForeignKey(nameof(TrainerId))]
	public virtual Trainer Trainer { get; set; } = null!;
}

public class LikeEntityConfiguration: IEntityTypeConfiguration<Like>
{
	public void Configure(EntityTypeBuilder<Like> builder)
	{
		builder.HasKey(l => new { l.CustomerId, l.TrainerId });
	}
}
