using AutoMapper;
using CoachSearch.Congiguration;
using CoachSearch.Data;
using CoachSearch.Models.Dto;
using CoachSearch.Models.Dto.TrainerProgram;
using CoachSearch.Services.FileUploadService;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace CoachSearch.Repositories.Trainer;

public class TrainerRepository(ApplicationDbContext dbContext, IFileUploadService fileUploadService): ITrainerRepository
{
	public Task SaveChangesAsync()
	{
		return dbContext.SaveChangesAsync();
	}

	public Task<Data.Entities.Trainer?> GetByIdAsync(long id)
	{
		return dbContext.Trainers.FirstOrDefaultAsync(t => t.TrainerId == id);
	}

	public async Task<bool> AddAsync(Data.Entities.Trainer trainer)
	{
		try
		{
			await dbContext.Trainers.AddAsync(trainer);
			await dbContext.SaveChangesAsync();
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
			var trainer = await dbContext.Trainers.FirstOrDefaultAsync(t => t.TrainerId == id);

			if (trainer == null)
				return false;
			
			pathTrainerDto.ApplyTo(trainer);
			await dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public IQueryable<Data.Entities.Trainer> GetAllByQuery()
	{
		return dbContext.Trainers;
	}

	public async Task<bool> UpdateAsync(long trainerId, TrainerProfileRequestDto profile)
	{
		try
		{
			var trainer = await dbContext.Trainers.FirstOrDefaultAsync(t => t.TrainerId == trainerId);

			if (trainer == null)
				return false;

			trainer.FirstName = profile.FirstName;
			trainer.MiddleName = profile.MiddleName;
			trainer.LastName = profile.LastName;
			trainer.City = profile.City;
			trainer.Specialization = profile.Specialization;
			/*trainer.TrainingPrograms = profile.TrainingPrograms.Select(t =>
				new Data.Entities.TrainingProgram()
				{
					TrainingProgramName = t.TrainingProgramName,
					TrainingProgramPrice = t.TrainingProgramPrice
				}).ToList();*/
			trainer.Info = profile.Info;
			trainer.TelegramLink = profile.TelegramLink;
			trainer.InstagramLink = profile.InstagramLink;
			
			if (trainer.AvatarFileName != null)
				fileUploadService.DeleteFile(trainer.AvatarFileName);

			trainer.AvatarFileName = profile.Avatar != null
				? await fileUploadService.UploadFileAsync(profile.Avatar)
				: null;
			
			await dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public async Task<bool> UpdateTrainingProgramsAsync(long trainerId, List<TrainingProgramRequestDto> trainingPrograms)
	{
		try
		{
			var trainer = await dbContext.Trainers.FirstOrDefaultAsync(t => t.TrainerId == trainerId);
			if (trainer == null)
				return false;

			trainer.TrainingPrograms = trainingPrograms.Select(t => new Data.Entities.TrainingProgram()
			{
				TrainingProgramName = t.TrainingProgramName,
				TrainingProgramPrice = t.TrainingProgramPrice
			}).ToList();

			await dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}
}