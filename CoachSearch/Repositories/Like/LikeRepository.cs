using CoachSearch.Data;
 using Microsoft.EntityFrameworkCore;
 
 namespace CoachSearch.Repositories.Like;
 
 public class LikeRepository(ApplicationDbContext dbContext): ILikeRepository
 {
 	public async Task<bool> ToggleLike(Data.Entities.Customer customer, Data.Entities.Trainer trainer)
    {
	    try
	    {
		    var existingLike = await dbContext.Likes.FirstOrDefaultAsync(l =>
			    l.TrainerId == trainer.TrainerId && l.CustomerId == customer.CustomerId);

		    if (existingLike == null)
		    {
			    var newLike = new Data.Entities.Like()
			    {
				    Customer = customer,
				    Trainer = trainer
			    };
			    dbContext.Likes.Add(newLike);
		    }
		    else
		    {
			    dbContext.Likes.Remove(existingLike);
		    }

		    await dbContext.SaveChangesAsync();
		    return true;
	    }
	    catch
	    {
		    return false;
	    }
    }
 }