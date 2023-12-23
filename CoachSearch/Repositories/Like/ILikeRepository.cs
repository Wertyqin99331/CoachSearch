namespace CoachSearch.Repositories.Like;

public interface ILikeRepository
{
	Task<bool> ToggleLike(Data.Entities.Customer customer, Data.Entities.Trainer trainer);
}