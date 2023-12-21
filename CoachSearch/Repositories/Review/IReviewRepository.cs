namespace CoachSearch.Repositories.Review;

public interface IReviewRepository
{
	public Task<bool> AddReviewAsync(string reviewTitle, string reviewText, DateOnly reviewDate,
		Data.Entities.Customer customer, Data.Entities.Trainer trainer);
}