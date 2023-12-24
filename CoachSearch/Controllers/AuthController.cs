using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CoachSearch.Data;
using CoachSearch.Data.Entities;
using CoachSearch.Extensions;
using CoachSearch.Models;
using CoachSearch.Models.Dto;
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
	/// <summary>
	/// Register a user
	/// </summary>
	/// <param name="registrationRequestDto"></param>
	/// <returns></returns>
	[HttpPost("register")]
	[ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[EndpointDescription("Register a user")]
	public async Task<IActionResult> Register([FromForm] RegistrationRequestDto registrationRequestDto)
	{
		if (registrationRequestDto.Email is null && registrationRequestDto.PhoneNumber is null)
			return BadRequest(new ResponseError("Not enough information to register"));
		
		if (registrationRequestDto.Email is not null)
		{
			if (await userManager.FindByEmailAsync(registrationRequestDto.Email) is not null)
				return BadRequest(new ResponseError("User with this email already exists"));
		}
		
		if (registrationRequestDto.PhoneNumber is not null)
		{
			if (await userManager.FindByPhoneNumberAsync(registrationRequestDto.PhoneNumber!) is not null)
				return BadRequest(new ResponseError("User with this phone already exists"));
		}
		
		var roleInString = registrationRequestDto.UserRole.ToString();
		if (await roleManager.FindByNameAsync(roleInString) is null)
			return BadRequest(new ResponseError("There is no role with this name"));

		var user = new ApplicationUser()
		{
			Email = registrationRequestDto.Email,
			PhoneNumber = registrationRequestDto.PhoneNumber,
			UserName = registrationRequestDto.Email ?? registrationRequestDto.PhoneNumber
		};

		var result = await userManager.CreateAsync(user, registrationRequestDto.Password);

		if (!result.Succeeded)
			return StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
		
		var registeredUser = await userManager.FindByIdAsync(user.Id);
		await userManager.AddToRoleAsync(registeredUser!, roleInString);

		/*
		if (registrationRequestDto.UserRole == UserRole.Customer)
		{
			var customer = new Customer() { UserInfo = registeredUser! };
			await customerRepository.AddAsync(customer);
		}
		else
		{
			var trainer = new Trainer() { UserInfo = registeredUser! };
			await trainerRepository.AddAsync(trainer);
		}*/

		return await Login(new LoginRequestDto()
			{ Email = registeredUser?.Email, PhoneNumber = registeredUser?.PhoneNumber, Password = registrationRequestDto.Password });
	}
	
	/// <summary>
	/// Login by email and password
	/// </summary>
	/// <param name="loginRequestDto"></param>
	/// <returns></returns>
	[HttpPost("login")]
	[ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Login([FromForm] LoginRequestDto loginRequestDto)
	{
		if (loginRequestDto.Email is null && loginRequestDto.PhoneNumber is null)
			return BadRequest(new ResponseError("Not enough information to register"));

		var managedUser = await userManager.FindByCredentialsAsync(loginRequestDto.Email, loginRequestDto.PhoneNumber);

		if (managedUser == null)
			return BadRequest(new ResponseError("There is no user with these credentials"));

		var isPasswordValid = await userManager.CheckPasswordAsync(managedUser, loginRequestDto.Password);

		if (!isPasswordValid)
			return BadRequest(new ResponseError("Password was wrong"));

		var userRoles = await userManager.GetRolesAsync(managedUser);
		var jwtToken = tokenService.CreateToken(managedUser, userRoles);

		return Ok(new LoginResponseDto()
		{
			Token = jwtToken
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
	public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordRequestDto body)
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