using CoachSearch.Data.Entities;
using CoachSearch.Extensions;
using CoachSearch.Models;
using CoachSearch.Models.Dto.ProfileDto;
using CoachSearch.Models.Enums;
using CoachSearch.Repositories.Customer;
using CoachSearch.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoachSearch.Controllers;

[ApiController]
[Route("/api/customer")]
public class CustomerController(IUserService userService, 
	UserManager<ApplicationUser> userManager,
	ICustomerRepository customerRepository) : Controller
{
	/// <summary>
	/// Get a customer profile
	/// </summary>
	/// <returns></returns>
	[Authorize(Roles = "Customer")]
	[HttpGet("profile")]
	[ProducesResponseType<CustomerProfileResponseDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetCustomerProfile()
	{
		var (email, phoneNumber) = userService.GetCredentials();

		if (email is null && phoneNumber is null)
			return BadRequest(new ResponseError("Can't read credentials from jwt"));

		var user = await userManager.FindByCredentialsAsync(email, phoneNumber);

		if (user is null)
			return BadRequest("There is no user with these credentials");

		var userRole = userService.GetUserRole();

		if (userRole == UserRole.Customer)
		{
			var customerInfo = user.Customer;

			if (customerInfo == null)
				return BadRequest(new ResponseError("There is no customer associated with user"));

			return Ok(new CustomerProfileResponseDto()
			{
				CustomerId = customerInfo.CustomerId,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				FirstName = customerInfo.FirstName,
				MiddleName = customerInfo.MiddleName,
				LastName = customerInfo.LastName,
				City = customerInfo.City,
				Info = customerInfo.Info
			});
		}

		return BadRequest(new ResponseError("Can't read user role from jwt"));
	}
	
	/// <summary>
	/// Update a customer profile
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	[Authorize(Roles = "Customer")]
	[HttpPost("profile")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UpdateCustomerProfile([FromBody] CustomerProfileRequestDto body)
	{
		var (email, phoneNumber) = userService.GetCredentials();
		if (email == null && phoneNumber == null)
			return BadRequest(new ResponseError("Can't read credentials from jwt"));

		var user = await userManager.FindByCredentialsAsync(email, phoneNumber);
		if (user == null)
			return BadRequest(new ResponseError("There is no user with these credentials"));

		var customerInfo = user.Customer;
		if (customerInfo == null)
		{
			var newCustomerInfo = new Customer()
			{
				FirstName = body.FirstName,
				MiddleName = body.MiddleName,
				LastName = body.LastName,
				City = body.City,
				UserInfo = user,
				Info = body.Info,
			};

			var result = await customerRepository.AddAsync(newCustomerInfo);
			return result
				? NoContent()
				: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
		}
		else
		{
			var result = await customerRepository.UpdateAsync(customerInfo.CustomerId, body);
			return result
				? NoContent()
				: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
		}
	}
}