using CoachSearch.Data;

namespace CoachSearch.Repositories.Review;

public class ReviewRepository: IReviewRepository
{
	private readonly ApplicationDbContext _dbContext;

	public ReviewRepository(ApplicationDbContext dbContext)
	{
		this._dbContext = dbContext;
	}
	
	public async Task<Data.Entities.Review?> AddReviewAsync(string reviewText, DateTime reviewDate, Data.Entities.Customer customer, Data.Entities.Trainer trainer)
	{
		try
		{
			var newReview = new Data.Entities.Review()
			{
				ReviewText = reviewText,
				ReviewDate = reviewDate,
				Customer = customer,
				Trainer = trainer
			};
			var result = await _dbContext.Reviews.AddAsync(newReview);
			await _dbContext.SaveChangesAsync();
			return result.Entity;
		}
		catch
		{
			return null;
		}
	}
}