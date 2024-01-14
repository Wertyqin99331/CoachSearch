using CoachSearch.Data;
 using Microsoft.EntityFrameworkCore;
 
 namespace CoachSearch.Repositories.Like;
 
 public class LikeRepository: ILikeRepository
 {
	 private readonly ApplicationDbContext _dbContext;

	 public LikeRepository(ApplicationDbContext dbContext)
	 {
		 _dbContext = dbContext;
	 }

	 public async Task<bool> ToggleLike(Data.Entities.Customer customer, Data.Entities.Trainer trainer)
    {
	    try
	    {
		    var existingLike = await _dbContext.Likes.FirstOrDefaultAsync(l =>
			    l.TrainerId == trainer.TrainerId && l.CustomerId == customer.CustomerId);

		    if (existingLike == null)
		    {
			    var newLike = new Data.Entities.Like()
			    {
				    Customer = customer,
				    Trainer = trainer
			    };
			    _dbContext.Likes.Add(newLike);
		    }
		    else
		    {
			    _dbContext.Likes.Remove(existingLike);
		    }

		    await _dbContext.SaveChangesAsync();
		    return true;
	    }
	    catch
	    {
		    return false;
	    }
    }
 }