using System.ComponentModel.DataAnnotations;
using CoachSearch.Data.Entities;
using CoachSearch.Extensions;
using CoachSearch.Models;
using CoachSearch.Models.Dto;
using CoachSearch.Models.Dto.Auth;
using CoachSearch.Models.Enums;
using CoachSearch.Repositories.Customer;
using CoachSearch.Repositories.Trainer;
using CoachSearch.Services.FileUploadService;
using CoachSearch.Services.Token;
using CoachSearch.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoachSearch.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : Controller
{
	public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<long>> roleManager,
		ITokenService tokenService, ICustomerRepository customerRepository, ITrainerRepository trainerRepository,
		IUserService userService, IFileUploadService fileUploadService)
	{
		this._userManager = userManager;
		this._roleManager = roleManager;
		this._tokenService = tokenService;
		this._customerRepository = customerRepository;
		this._trainerRepository = trainerRepository;
		this._userService = userService;
		this._fileUploadService = fileUploadService;
	}

	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole<long>> _roleManager;
	private readonly ITokenService _tokenService;
	private readonly ICustomerRepository _customerRepository;
	private readonly ITrainerRepository _trainerRepository;
	private readonly IUserService _userService;
	private readonly IFileUploadService _fileUploadService;

	/// <summary>
	/// Register a customer
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	[HttpPost("register/customer")]
	[ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[EndpointDescription("Register a customer")]
	public async Task<IActionResult> RegisterCustomer([FromForm] CustomerRegistrationDto body)
	{
		if (body.Email == null && body.PhoneNumber == null)
			return BadRequest(new ResponseError("Not enough information to register"));

		if (body.Email != null)
		{
			if (await this._userManager.FindByEmailAsync(body.Email) != null)
				return BadRequest(new ResponseError("User with this email already exists"));
		}

		if (body.PhoneNumber != null)
		{
			if (await this._userManager.FindByPhoneNumberAsync(body.PhoneNumber) != null)
				return BadRequest(new ResponseError("User with this phone already exists"));
		}

		const string roleInString = "Customer";
		if (await this._roleManager.FindByNameAsync(roleInString) == null)
			return BadRequest(new ResponseError("There is no role with this name"));

		var user = new ApplicationUser()
		{
			Email = body.Email,
			PhoneNumber = body.PhoneNumber,
			UserName = body.Email ?? body.PhoneNumber
		};

		var result = await this._userManager.CreateAsync(user, body.Password);

		if (!result.Succeeded)
			return StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));

		var registeredUser = await this._userManager.FindByIdAsync(user.Id);
		await this._userManager.AddToRoleAsync(registeredUser!, roleInString);

		var registeredCustomer = await body.ToCustomer(registeredUser!, this._fileUploadService);

		var customerAddingResult = await this._customerRepository.AddAsync(registeredCustomer);
		if (!customerAddingResult)
			return StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));

		return await Login(new LoginRequestDto()
			{ Login = body.Email ?? body.PhoneNumber!, Password = body.Password });
	}

	/// <summary>
	/// Register a trainer
	/// </summary>ч
	/// <param name="body"></param>
	/// <returns></returns>
	[HttpPost("register/trainer")]
	[ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[EndpointDescription("Register a customer")]
	public async Task<IActionResult> RegisterTrainer([FromForm] TrainerRegistrationDto body)
	{
		if (body.Email == null && body.PhoneNumber == null)
			return BadRequest(new ResponseError("Not enough information to register"));

		if (body.Email != null)
		{
			if (await this._userManager.FindByEmailAsync(body.Email) != null)
				return BadRequest(new ResponseError("User with this email already exists"));
		}

		if (body.PhoneNumber != null)
		{
			if (await this._userManager.FindByPhoneNumberAsync(body.PhoneNumber) != null)
				return BadRequest(new ResponseError("User with this phone already exists"));
		}

		const string roleInString = "Trainer";
		if (await this._roleManager.FindByNameAsync(roleInString) == null)
			return BadRequest(new ResponseError("There is no role with this name"));

		var user = new ApplicationUser()
		{
			Email = body.Email,
			PhoneNumber = body.PhoneNumber,
			UserName = body.Email ?? body.PhoneNumber
		};

		var result = await this._userManager.CreateAsync(user, body.Password);

		if (!result.Succeeded)
			return StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));

		var registeredUser = await this._userManager.FindByIdAsync(user.Id);
		await this._userManager.AddToRoleAsync(registeredUser!, roleInString);

		var registeredTrainer = await body.ToTrainer(registeredUser!, this._fileUploadService);
		
		var trainerAddingResult = await this._trainerRepository.AddAsync(registeredTrainer);
		if (!trainerAddingResult)
			return StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));

		return await Login(new LoginRequestDto()
			{ Login = body.Email ?? body.PhoneNumber!, Password = body.Password });
	}

	/// <summary>
	/// Login by email and password
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	[HttpPost("login")]
	[ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Login([FromForm] LoginRequestDto body)
	{
		var email = new EmailAddressAttribute().IsValid(body.Login)
			? body.Login
			: null;
		var phoneNumber = body.Login.Length == 11 && body.Login.All(char.IsNumber)
			? body.Login
			: null;

		var managedUser = await this._userManager.FindByCredentialsAsync(email, phoneNumber);

		if (managedUser == null)
			return BadRequest(new ResponseError("There is no user with these credentials"));
		
		if (!await this._userManager.CheckPasswordAsync(managedUser, body.Password))
			return BadRequest(new ResponseError("Password was wrong"));

		var userRoles = await this._userManager.GetRolesAsync(managedUser);
		var jwtToken = this._tokenService.CreateToken(managedUser, userRoles);

		return Ok(new LoginResponseDto()
		{
			Token = jwtToken,
			Role = managedUser.Trainer != null
				? UserRole.Trainer.ToString()
				: UserRole.Customer.ToString()
		});
	}

	/// <summary>
	///  Change a password of user
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	[Authorize]
	[HttpPost("change-password")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto body)
	{
		var (email, phoneNumber) = this._userService.GetCredentials();
		var user = await this._userManager.FindByCredentialsAsync(email, phoneNumber);

		if (user is null)
			return BadRequest(new ResponseError("There is no user with these credentials"));

		var changeResult = await this._userManager.ChangePasswordAsync(user, body.CurrentPassword, body.NewPassword);

		if (changeResult.Succeeded)
			return Ok();

		return BadRequest(new ResponseError("Something goes wrong"));
	}
}