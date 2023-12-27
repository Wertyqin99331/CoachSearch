using CoachSearch.Data;
using CoachSearch.Models.Dto.ProfileDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace CoachSearch.Repositories.Customer;

public class CustomerRepository(ApplicationDbContext dbContext): ICustomerRepository
{
	public async Task<bool> AddAsync(Data.Entities.Customer customer)
	{
		try
		{
			await dbContext.Customers.AddAsync(customer);
			await dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public async Task<bool> PatchAsync(long id, JsonPatchDocument patchCustomerDto)
	{
		try
		{
			var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);

			if (customer == null)
				return false;
			
			patchCustomerDto.ApplyTo(customer);
			await dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public async Task<bool> UpdateAsync(long customerId, CustomerProfileRequestDto profile)
	{
		try
		{
			var customer = await dbContext.Customers.FirstOrDefaultAsync(t => t.CustomerId == customerId);

			if (customer == null)
				return false;

			customer.FullName = profile.FullName;
			customer.Info = profile.Info;
			customer.VkLink = profile.VkLink;
			customer.TelegramLink = profile.TelegramLink;

			await dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}
}