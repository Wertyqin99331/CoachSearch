using CoachSearch.Data;
using CoachSearch.Models.Dto.TrainerProgram;
using CoachSearch.Repositories.Trainer;
using Microsoft.EntityFrameworkCore;

namespace CoachSearch.Repositories.TrainingProgram;

public class TrainingProgramRepository(ApplicationDbContext dbContext, ITrainerRepository trainerRepository): ITrainingProgramRepository
{
	public Task<Data.Entities.TrainingProgram?> GetByIdAsync(long trainingProgramId)
	{
		return dbContext.TrainingPrograms.FirstOrDefaultAsync(t => t.TrainingProgramId == trainingProgramId);
	}

	public IQueryable<Data.Entities.TrainingProgram> GetAllByTrainerIdQueryable(long trainerId)
	{
		return dbContext.TrainingPrograms.Where(tp => tp.TrainerId == trainerId);
	}

	public IQueryable<TrainingProgramDto> GetAllByTrainerIdToDto(long trainerId)
	{
		return GetAllByTrainerIdQueryable(trainerId)
			.Select(tp => new TrainingProgramDto()
			{
				TrainingProgramName = tp.TrainingProgramName,
				TrainingProgramPrice = tp.TrainingProgramPrice
			});
	}

	public async Task<bool> AddTrainingProgramsByTrainerIdAsync(long trainerId, IEnumerable<Data.Entities.TrainingProgram> programs)
	{
		try
		{
			var trainer = await trainerRepository.GetByIdAsync(trainerId);
			if (trainer == null)
				return false;
		
			trainer.TrainingPrograms.AddRange(programs);
			await dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
		
	}
}