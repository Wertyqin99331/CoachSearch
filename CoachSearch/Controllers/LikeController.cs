using CoachSearch.Data.Entities;
using CoachSearch.Extensions;
using CoachSearch.Models;
using CoachSearch.Models.Dto.Like;
using CoachSearch.Repositories.Customer;
using CoachSearch.Repositories.Like;
using CoachSearch.Repositories.Trainer;
using CoachSearch.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoachSearch.Controllers;

[ApiController]
[Route("api/like")]
public class LikeController : Controller
{
	public LikeController(
		IUserService userService,
		UserManager<ApplicationUser> userManager,
		ITrainerRepository trainerRepository,
		ILikeRepository likeRepository)
	{
		_userService = userService;
		_userManager = userManager;
		_trainerRepository = trainerRepository;
		_likeRepository = likeRepository;
	}
	
	private readonly IUserService _userService;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly ITrainerRepository _trainerRepository;
	private readonly ILikeRepository _likeRepository;
	/// <summary>
	/// Toggle user like to a trainer
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	[Authorize(Roles = "Customer")]
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ResponseError), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> ToggleLike([FromBody] ToggleLikeRequestDto body)
	{
		var (email, phoneNumber) = _userService.GetCredentials();
		if (email == null && phoneNumber == null)
			return BadRequest(new ResponseError("No email and phone number in your jwt"));

		var user = await _userManager.FindByCredentialsAsync(email, phoneNumber);
		if (user == null)
			return BadRequest(new ResponseError("There is no user with these credentials"));

		var customer = user.Customer;
		if (customer == null)
			return BadRequest(new ResponseError("There is no customer associated with this user"));

		var trainer = await _trainerRepository.GetByIdAsync(body.TrainerId);
		if (trainer == null)
			return BadRequest(new ResponseError("There is no trainer with this id)"));

		var result = await _likeRepository.ToggleLike(customer, trainer);
		return result
			? NoContent()
			: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
	}
}