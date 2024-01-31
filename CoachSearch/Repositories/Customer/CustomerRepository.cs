using CoachSearch.Data;
using CoachSearch.Models.Dto.Customer;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace CoachSearch.Repositories.Customer;

public class CustomerRepository: ICustomerRepository
{
	private readonly ApplicationDbContext _dbContext;
	
	public CustomerRepository(ApplicationDbContext dbContext)
	{
		this._dbContext = dbContext;
	}
	
	public async Task<bool> AddAsync(Data.Entities.Customer customer)
	{
		try
		{
			await this._dbContext.Customers.AddAsync(customer);
			await this._dbContext.SaveChangesAsync();
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
			var customer = await this._dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);

			if (customer == null)
				return false;
			
			patchCustomerDto.ApplyTo(customer);
			await this._dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public async Task<bool> UpdateAsync(Data.Entities.Customer customer)
	{
		try
		{
			var existingCustomer =
				await this._dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId);
			
			if (existingCustomer == null)
				return false;
			
			this._dbContext.Customers.Entry(existingCustomer).CurrentValues.SetValues(customer);
			await this._dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}

	/*public async Task<bool> UpdateAsync(long customerId, CustomerProfileUpdateDto profile)
	{
		try
		{
			var customer = await this._dbContext.Customers.FirstOrDefaultAsync(t => t.CustomerId == customerId);

			if (customer == null)
				return false;

			customer.FullName = profile.FullName;
			customer.Info = profile.Info;
			customer.VkLink = profile.VkLink;
			customer.TelegramLink = profile.TelegramLink;

			await this._dbContext.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}*/
}