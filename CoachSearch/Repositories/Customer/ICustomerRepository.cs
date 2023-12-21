using CoachSearch.Models.Dto.ProfileDto;
using Microsoft.AspNetCore.JsonPatch;

namespace CoachSearch.Repositories.Customer;

public interface ICustomerRepository
{
	Task<bool> AddAsync(Data.Entities.Customer customer);
	Task<bool> PatchAsync(long id, JsonPatchDocument patchCustomerDto);
	Task<bool> UpdateAsync(long customerId, CustomerProfileRequestDto body);
}