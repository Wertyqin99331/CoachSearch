using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CoachSearch.Data;
using CoachSearch.Data.Entities;
using CoachSearch.Extensions;
using CoachSearch.Models;
using CoachSearch.Models.Dto;
using CoachSearch.Models.Dto.Review;
using CoachSearch.Models.Enums;
using CoachSearch.Repositories.Customer;
using CoachSearch.Repositories.Trainer;
using CoachSearch.Services.Token;
using CoachSearch.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoachSearch.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(
	UserManager<ApplicationUser> userManager,
	RoleManager<IdentityRole<long>> roleManager,
	ITokenService tokenService,
	ICustomerRepository customerRepository, 
	ITrainerRepository trainerRepository,
	IUserService userService) : Controller
{
	/*/// <summary>
	/// Register a user
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	[HttpPost("register")]
	[ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[EndpointDescription("Register a user")]
	public async Task<IActionResult> Register([FromBody] RegistrationRequestDto body)
	{
		if (body.Email is null && body.PhoneNumber is null)
			return BadRequest(new ResponseError("Not enough information to register"));
		
		if (body.Email is not null)
		{
			if (await userManager.FindByEmailAsync(body.Email) is not null)
				return BadRequest(new ResponseError("User with this email already exists"));
		}
		
		if (body.PhoneNumber is not null)
		{
			if (await userManager.FindByPhoneNumberAsync(body.PhoneNumber!) is not null)
				return BadRequest(new ResponseError("User with this phone already exists"));
		}
		
		var roleInString = body.UserRole.ToString();
		if (await roleManager.FindByNameAsync(roleInString) is null)
			return BadRequest(new ResponseError("There is no role with this name"));

		var user = new ApplicationUser()
		{
			Email = body.Email,
			PhoneNumber = body.PhoneNumber,
			UserName = body.Email ?? body.PhoneNumber
		};

		var result = await userManager.CreateAsync(user, body.Password);

		if (!result.Succeeded)
			return StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
		
		var registeredUser = await userManager.FindByIdAsync(user.Id);
		await userManager.AddToRoleAsync(registeredUser!, roleInString);

		/*
		if (body.UserRole == UserRole.Customer)
		{
			var customer = new Customer() { UserInfo = registeredUser! };
			await customerRepository.AddAsync(customer);
		}
		else
		{
			var trainer = new Trainer() { UserInfo = registeredUser! };
			await trainerRepository.AddAsync(trainer);
		}#1#

		return await Login(new LoginRequestDto()
			{ Email = registeredUser?.Email, PhoneNumber = registeredUser?.PhoneNumber, Password = body.Password });
	}*/
	
	/// <summary>
	/// Register a customer
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	[HttpPost("register/customer")]
	[ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[EndpointDescription("Register a customer")]
	public async Task<IActionResult> RegisterCustomer([FromBody] RegistrationCustomerRequestDto body)
	{
		if (body.Email is null && body.PhoneNumber is null)
			return BadRequest(new ResponseError("Not enough information to register"));
		
		if (body.Email is not null)
		{
			if (await userManager.FindByEmailAsync(body.Email) is not null)
				return BadRequest(new ResponseError("User with this email already exists"));
		}
		
		if (body.PhoneNumber is not null)
		{
			if (await userManager.FindByPhoneNumberAsync(body.PhoneNumber!) is not null)
				return BadRequest(new ResponseError("User with this phone already exists"));
		}

		const string roleInString = "Customer";
		if (await roleManager.FindByNameAsync(roleInString) is null)
			return BadRequest(new ResponseError("There is no role with this name"));

		var user = new ApplicationUser()
		{
			Email = body.Email,
			PhoneNumber = body.PhoneNumber,
			UserName = body.Email ?? body.PhoneNumber
		};

		var result = await userManager.CreateAsync(user, body.Password);

		if (!result.Succeeded)
			return StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
		
		var registeredUser = await userManager.FindByIdAsync(user.Id);
		await userManager.AddToRoleAsync(registeredUser!, roleInString);
		
		var registeredCustomer = new Customer()
		{
			FullName = body.FullName,
			Info = body.Info,
			TelegramLink = body.TelegramLink,
			VkLink = body.VkLink,
			UserInfo = registeredUser!
		};

		var customerAddingResult = await customerRepository.AddAsync(registeredCustomer);
		if (!customerAddingResult)
			return StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));

		return await Login(new LoginRequestDto()
			{ Login = body.Email ?? body.PhoneNumber!, Password = body.Password, Role = UserRole.Customer});
	}
	
	/// <summary>
	/// Register a trainer
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	[HttpPost("register/trainer")]
	[ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[EndpointDescription("Register a customer")]
	public async Task<IActionResult> RegisterTrainer([FromBody] TrainerRegistrationRequestDto body)
	{
		if (body.Email is null && body.PhoneNumber is null)
			return BadRequest(new ResponseError("Not enough information to register"));
		
		if (body.Email is not null)
		{
			if (await userManager.FindByEmailAsync(body.Email) is not null)
				return BadRequest(new ResponseError("User with this email already exists"));
		}
		
		if (body.PhoneNumber is not null)
		{
			if (await userManager.FindByPhoneNumberAsync(body.PhoneNumber!) is not null)
				return BadRequest(new ResponseError("User with this phone already exists"));
		}

		const string roleInString = "Trainer";
		if (await roleManager.FindByNameAsync(roleInString) is null)
			return BadRequest(new ResponseError("There is no role with this name"));

		var user = new ApplicationUser()
		{
			Email = body.Email,
			PhoneNumber = body.PhoneNumber,
			UserName = body.Email ?? body.PhoneNumber
		};

		var result = await userManager.CreateAsync(user, body.Password);

		if (!result.Succeeded)
			return StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
		
		var registeredUser = await userManager.FindByIdAsync(user.Id);
		await userManager.AddToRoleAsync(registeredUser!, roleInString);
		
		var registeredTrainer = new Trainer()
		{
			FullName = body.FullName,
			Info = body.Info,
			TelegramLink = body.TelegramLink,
			VkLink = body.VkLink,
			UserInfo = registeredUser!,
			Specialization = body.Specialization,
			AvatarFileName = null
		};

		var trainerAddingResult = await trainerRepository.AddAsync(registeredTrainer);
		if (!trainerAddingResult)
			return StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));

		return await Login(new LoginRequestDto()
			{ Login = body.Email ?? body.PhoneNumber!, Password = body.Password, Role = UserRole.Trainer});
	}
	
	/// <summary>
	/// Login by email and password
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	[HttpPost("login")]
	[ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Login([FromBody] LoginRequestDto body)
	{
		var email = new EmailAddressAttribute().IsValid(body.Login) 
			? body.Login 
			: null;
		var phoneNumber = body.Login.Length == 11 && body.Login.All(char.IsNumber)
			? body.Login
			: null;
		
		var managedUser = await userManager.FindByCredentialsAsync(email, phoneNumber);

		if (managedUser == null)
			return BadRequest(new ResponseError("There is no user with these credentials"));

		var isPasswordValid = await userManager.CheckPasswordAsync(managedUser, body.Password);

		if (!isPasswordValid)
			return BadRequest(new ResponseError("Password was wrong"));

		var userRoles = await userManager.GetRolesAsync(managedUser);
		var jwtToken = tokenService.CreateToken(managedUser, userRoles);

		return Ok(new LoginResponseDto()
		{
			Token = jwtToken,
			Role = body.Role
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
		var (email, phoneNumber) = userService.GetCredentials();
		var user = await userManager.FindByCredentialsAsync(email, phoneNumber);

		if (user is null)
			return BadRequest(new ResponseError("There is no user with these credentials"));

		var changeResult = await userManager.ChangePasswordAsync(user, body.CurrentPassword, body.NewPassword);

		if (changeResult.Succeeded)
			return Ok();
		
		return BadRequest(new ResponseError("Something goes wrong"));
	}
}