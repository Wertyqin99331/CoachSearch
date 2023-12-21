using CoachSearch.Data;

namespace CoachSearch.Repositories.Review;

public class ReviewRepository(ApplicationDbContext dbContext): IReviewRepository
{
	public async Task<bool> AddReviewAsync(string reviewTitle, string reviewText, DateOnly reviewDate, Data.Entities.Customer customer, Data.Entities.Trainer trainer)
	{
		try
		{
			var newReview = new Data.Entities.Review()
			{
				ReviewTitle = reviewTitle,
				ReviewText = reviewText,
				ReviewDate = reviewDate,
				Customer = customer,
				Trainer = trainer
			};
			await dbContext.AddAsync(newReview);
			await dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}
}