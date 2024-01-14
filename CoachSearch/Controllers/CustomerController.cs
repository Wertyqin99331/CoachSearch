using CoachSearch.Data.Entities;
using CoachSearch.Extensions;
using CoachSearch.Models;
using CoachSearch.Models.Dto.ProfileDto;
using CoachSearch.Models.Enums;
using CoachSearch.Repositories.Customer;
using CoachSearch.Services.FileUploadService;
using CoachSearch.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoachSearch.Controllers;

[ApiController]
[Route("/api/customer")]
public class CustomerController: Controller
{
	public CustomerController(IUserService userService,
		UserManager<ApplicationUser> userManager,
		ICustomerRepository customerRepository,
		IFileUploadService fileUploadService)
	{
		_userService = userService;
		_userManager = userManager;
		_customerRepository = customerRepository;
		_fileUploadService = fileUploadService;
	}

	private readonly IUserService _userService;

	private readonly UserManager<ApplicationUser> _userManager;

	private readonly ICustomerRepository _customerRepository;

	private readonly IFileUploadService _fileUploadService;

	/// <summary>
	/// Get a customer profile
	/// </summary>
	/// <returns></returns>
	[Authorize(Roles = "Customer")]
	[HttpGet("profile")]
	[ProducesResponseType(typeof(CustomerProfileResponseDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetCustomerProfile()
	{
		var (email, phoneNumber) = _userService.GetCredentials();

		if (email is null && phoneNumber is null)
			return BadRequest(new ResponseError("Can't read credentials from jwt"));

		var user = await _userManager.FindByCredentialsAsync(email, phoneNumber);

		if (user is null)
			return BadRequest("There is no user with these credentials");

		var userRole = _userService.GetUserRole();

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
				FullName = customerInfo.FullName,
				Info = customerInfo.Info,
				TelegramLink = customerInfo.TelegramLink,
				VkLink = customerInfo.VkLink,
				AvatarUrl = _fileUploadService.GetAvatarUrl(Request, customerInfo.AvatarFileName)
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
	[ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UpdateCustomerProfile([FromBody] CustomerProfileRequestDto body)
	{
		var (email, phoneNumber) = _userService.GetCredentials();
		if (email == null && phoneNumber == null)
			return BadRequest(new ResponseError("Can't read credentials from jwt"));

		var user = await _userManager.FindByCredentialsAsync(email, phoneNumber);
		if (user == null)
			return BadRequest(new ResponseError("There is no user with these credentials"));

		var customerInfo = user.Customer;
		if (customerInfo == null)
		{
			var newCustomerInfo = new Customer()
			{
				FullName = body.FullName,
				UserInfo = user,
				Info = body.Info,
				TelegramLink = body.TelegramLink,
				VkLink = body.VkLink,
				AvatarFileName = null
				// Todo 
			};

			var result = await _customerRepository.AddAsync(newCustomerInfo);
			return result
				? NoContent()
				: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
		}
		else
		{
			var result = await _customerRepository.UpdateAsync(customerInfo.CustomerId, body);
			return result
				? NoContent()
				: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
		}
	}
}