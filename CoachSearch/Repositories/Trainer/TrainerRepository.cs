﻿using AutoMapper;
using CoachSearch.Data;
using CoachSearch.Models.Dto;
using CoachSearch.Models.Dto.TrainerProgram;
using CoachSearch.Services.FileUploadService;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace CoachSearch.Repositories.Trainer;

public class TrainerRepository: ITrainerRepository
{
	private readonly ApplicationDbContext _dbContext;
	private readonly IFileUploadService _fileUploadService;

	public TrainerRepository(ApplicationDbContext dbContext, IFileUploadService fileUploadService)
	{
		this._dbContext = dbContext;
		this._fileUploadService = fileUploadService;
	}
	
	public Task SaveChangesAsync()
	{
		return _dbContext.SaveChangesAsync();
	}

	public Task<Data.Entities.Trainer?> GetByIdAsync(long id)
	{
		return _dbContext.Trainers.FirstOrDefaultAsync(t => t.TrainerId == id);
	}

	public async Task<bool> AddAsync(Data.Entities.Trainer trainer)
	{
		try
		{
			await _dbContext.Trainers.AddAsync(trainer);
			await _dbContext.SaveChangesAsync();
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
			var trainer = await _dbContext.Trainers.FirstOrDefaultAsync(t => t.TrainerId == id);

			if (trainer == null)
				return false;
			
			pathTrainerDto.ApplyTo(trainer);
			await _dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public IQueryable<Data.Entities.Trainer> GetAllByQuery()
	{
		return _dbContext.Trainers;
	}

	public async Task<bool> UpdateAsync(long trainerId, TrainerProfileRequestDto profile)
	{
		try
		{
			var trainer = await _dbContext.Trainers.FirstOrDefaultAsync(t => t.TrainerId == trainerId);

			if (trainer == null)
				return false;

			trainer.FullName = profile.FullName;
			trainer.Specialization = profile.Specialization;
			/*trainer.TrainingPrograms = profile.TrainingPrograms.Select(t =>
				new Data.Entities.TrainingProgram()
				{
					TrainingProgramName = t.TrainingProgramName,
					TrainingProgramPrice = t.TrainingProgramPrice
				}).ToList();*/
			trainer.Info = profile.Info;
			trainer.TelegramLink = profile.TelegramLink;
			trainer.VkLink = profile.VkLink;
			
			if (trainer.AvatarFileName != null)
				_fileUploadService.DeleteFile(trainer.AvatarFileName);

			trainer.AvatarFileName = profile.Avatar != null
				? await _fileUploadService.UploadFileAsync(profile.Avatar)
				: null;
			
			await _dbContext.SaveChangesAsync();
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