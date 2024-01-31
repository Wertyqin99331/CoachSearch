using CoachSearch.Data.Entities;
using CoachSearch.Extensions;
using CoachSearch.Models;
using CoachSearch.Models.Dto.Customer;
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
public class CustomerController : Controller
{
	public CustomerController(IUserService userService,
		UserManager<ApplicationUser> userManager,
		ICustomerRepository customerRepository,
		IFileUploadService fileUploadService)
	{
		this._userService = userService;
		this._userManager = userManager;
		this._customerRepository = customerRepository;
		this._fileUploadService = fileUploadService;
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
	[ProducesResponseType(typeof(CustomerProfileDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetCustomerProfile()
	{
		var (email, phoneNumber) = this._userService.GetCredentials();

		if (email == null && phoneNumber == null)
			return BadRequest(new ResponseError("Can't read credentials from jwt"));

		var user = await this._userManager.FindByCredentialsAsync(email, phoneNumber);

		if (user == null)
			return BadRequest("There is no user with these credentials");

		var userRole = this._userService.GetUserRole();

		if (userRole != UserRole.Customer) return BadRequest(new ResponseError("Can't read user role from jwt"));
		var customerInfo = user.Customer;

		if (customerInfo == null)
			return BadRequest(new ResponseError("There is no customer associated with user"));

		return Ok(CustomerProfileDto.FromCustomer(customerInfo, user, this._fileUploadService, this.Request));
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
	public async Task<IActionResult> UpdateCustomerProfile([FromForm] CustomerProfileUpdateDto body)
	{
		var (email, phoneNumber) = this._userService.GetCredentials();
		if (email == null && phoneNumber == null)
			return BadRequest(new ResponseError("Can't read credentials from jwt"));

		var user = await this._userManager.FindByCredentialsAsync(email, phoneNumber);
		if (user == null)
			return BadRequest(new ResponseError("There is no user with these credentials"));

		var customerInfo = user.Customer;
		if (customerInfo == null)
			return BadRequest(new ResponseError("There is no customer associated with this user"));

		var result = await this._customerRepository.UpdateAsync(await body.ToCustomer(customerInfo, this._fileUploadService));
		return result
			? NoContent()
			: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
	}
}