using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachSearch.Data.Entities;

public class TrainingProgram
{
	public long TrainingProgramId { get; set; }

	[Required] [MaxLength(50)] public string TrainingProgramName { get; set; } = null!;

	public int TrainingProgramPrice { get; set; }


	public long TrainerId { get; set; }
	[ForeignKey("TrainerId")] public virtual Trainer Trainer { get; set; } = null!;

	public virtual List<Review> Reviews { get; set; } = null!;
}