using CoachSearch.Models.Dto.TrainerProgram;

namespace CoachSearch.Repositories.TrainingProgram;

public interface ITrainingProgramRepository
{
	Task<CoachSearch.Data.Entities.TrainingProgram?> GetByIdAsync(long trainingProgramId);
	IQueryable<Data.Entities.TrainingProgram> GetAllByTrainerIdQueryable(long trainerId);
	IQueryable<TrainingProgramDto> GetAllByTrainerIdToDto(long trainerId);
	Task<bool> AddTrainingProgramsByTrainerIdAsync(long trainerId, IEnumerable<Data.Entities.TrainingProgram> programs);
}