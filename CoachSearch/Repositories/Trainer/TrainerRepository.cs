using CoachSearch.Data;
using CoachSearch.Models.Dto.TrainerProgram;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace CoachSearch.Repositories.Trainer;

public class TrainerRepository : ITrainerRepository
{
	private readonly ApplicationDbContext _dbContext;

	public TrainerRepository(ApplicationDbContext dbContext)
	{
		this._dbContext = dbContext;
	}

	public Task SaveChangesAsync()
	{
		return this._dbContext.SaveChangesAsync();
	}

	public Task<Data.Entities.Trainer?> GetByIdAsync(long id)
	{
		return this._dbContext.Trainers.FirstOrDefaultAsync(t => t.TrainerId == id);
	}

	public async Task<bool> AddAsync(Data.Entities.Trainer trainer)
	{
		try
		{
			await this._dbContext.Trainers.AddAsync(trainer);
			await this._dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public async Task<bool> PatchAsync(long id, JsonPatchDocument pathTrainerDto)
	{
		try
		{
			var trainer = await this._dbContext.Trainers.FirstOrDefaultAsync(t => t.TrainerId == id);

			if (trainer == null)
				return false;

			pathTrainerDto.ApplyTo(trainer);
			await this._dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public IQueryable<Data.Entities.Trainer> GetAllByQuery()
	{
		return this._dbContext.Trainers;
	}

	public async Task<bool> UpdateAsync(Data.Entities.Trainer updatedTrainer)
	{
		try
		{
			var existingTrainer =
				await this._dbContext.Trainers.FirstOrDefaultAsync(t => t.TrainerId == updatedTrainer.TrainerId);

			if (existingTrainer == null)
				return false;

			this._dbContext.Trainers.Entry(existingTrainer).CurrentValues.SetValues(updatedTrainer);
			await this._dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public async Task<bool> UpdateTrainingProgramsAsync(long trainerId,
		List<TrainingProgramRequestDto> trainingPrograms)
	{
		try
		{
			var trainer = await _dbContext.Trainers.FirstOrDefaultAsync(t => t.TrainerId == trainerId);
			if (trainer == null)
				return false;

			trainer.TrainingPrograms = trainingPrograms.Select(t => new Data.Entities.TrainingProgram()
			{
				TrainingProgramName = t.TrainingProgramName,
				TrainingProgramPrice = t.TrainingProgramPrice
			}).ToList();

			await _dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public Task<List<string>> GetAllAddresses()
	{
		return _dbContext.Trainers
			.Select(t => t.Address)
			.ToListAsync();
	}
}