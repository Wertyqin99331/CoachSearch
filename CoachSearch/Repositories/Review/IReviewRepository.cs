namespace CoachSearch.Repositories.Review;

public interface IReviewRepository
{
	public Task<Data.Entities.Review?> AddReviewAsync(string reviewText, DateTime reviewDate,
		Data.Entities.Customer customer, Data.Entities.Trainer trainer);
}