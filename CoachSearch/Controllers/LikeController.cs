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
public class LikeController(
	IUserService userService,
	UserManager<ApplicationUser> userManager,
	ITrainerRepository trainerRepository,
	ILikeRepository likeRepository,
	ICustomerRepository customerRepository) : Controller
{
	/// <summary>
	/// Toggle user like to a trainer
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	[Authorize(Roles = "Customer")]
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> ToggleLike([FromBody] ToggleLikeRequestDto body)
	{
		var (email, phoneNumber) = userService.GetCredentials();
		if (email == null && phoneNumber == null)
			return BadRequest(new ResponseError("No email and phone number in your jwt"));

		var user = await userManager.FindByCredentialsAsync(email, phoneNumber);
		if (user == null)
			return BadRequest(new ResponseError("There is no user with these credentials"));

		var customer = user.Customer;
		if (customer == null)
			return BadRequest(new ResponseError("There is no customer associated with this user"));

		var trainer = await trainerRepository.GetByIdAsync(body.TrainerId);
		if (trainer == null)
			return BadRequest(new ResponseError("There is no trainer with this id)"));

		var result = await likeRepository.ToggleLike(customer, trainer);
		return result
			? NoContent()
			: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
	}
}