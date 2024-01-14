using CoachSearch.Data;
using CoachSearch.Models.Dto.TrainerProgram;
using CoachSearch.Repositories.Trainer;
using Microsoft.EntityFrameworkCore;

namespace CoachSearch.Repositories.TrainingProgram;

public class TrainingProgramRepository: ITrainingProgramRepository
{
	private readonly ApplicationDbContext _dbContext;
	private readonly ITrainerRepository _trainerRepository;

	public TrainingProgramRepository(ApplicationDbContext dbContext, ITrainerRepository trainerRepository)
	{
		_dbContext = dbContext;
		_trainerRepository = trainerRepository;
	}
	
	public Task<Data.Entities.TrainingProgram?> GetByIdAsync(long trainingProgramId)
	{
		return _dbContext.TrainingPrograms.FirstOrDefaultAsync(t => t.TrainingProgramId == trainingProgramId);
	}

	public IQueryable<Data.Entities.TrainingProgram> GetAllByTrainerIdQueryable(long trainerId)
	{
		return _dbContext.TrainingPrograms.Where(tp => tp.TrainerId == trainerId);
	}

	public IQueryable<TrainingProgramDto> GetAllByTrainerIdToDto(long trainerId)
	{
		return GetAllByTrainerIdQueryable(trainerId)
			.Select(tp => new TrainingProgramDto()
			{
				TrainingProgramId = tp.TrainingProgramId,
				TrainingProgramName = tp.TrainingProgramName,
				TrainingProgramPrice = tp.TrainingProgramPrice
			});
	}

	public async Task<bool> AddTrainingProgramsByTrainerIdAsync(long trainerId, IEnumerable<Data.Entities.TrainingProgram> programs)
	{
		try
		{
			var trainer = await _trainerRepository.GetByIdAsync(trainerId);
			if (trainer == null)
				return false;
		
			trainer.TrainingPrograms.AddRange(programs);
			await _dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
		
	}
}