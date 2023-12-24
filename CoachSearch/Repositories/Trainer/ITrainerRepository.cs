using CoachSearch.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;

namespace CoachSearch.Repositories.Trainer;

public interface ITrainerRepository
{
	Task SaveChangesAsync();
	
	Task<Data.Entities.Trainer?> GetByIdAsync(long id);
	
	Task<bool> AddAsync(Data.Entities.Trainer trainer);
	Task<bool> PatchAsync(long id, JsonPatchDocument pathTrainerDto);

	IQueryable<Data.Entities.Trainer> GetAllByQuery();

	Task<bool> UpdateAsync(long trainerId, TrainerProfileRequestDto profile);
}