using CoachSearch.Data.Entities;
using CoachSearch.Extensions;
using CoachSearch.Models;
using CoachSearch.Models.Dto.Review;
using CoachSearch.Repositories.Review;
using CoachSearch.Repositories.Trainer;
using CoachSearch.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoachSearch.Controllers;

[ApiController]
[Route("review")]
public class ReviewController(
	IUserService userService,
	UserManager<ApplicationUser> userManager,
	ITrainerRepository trainerRepository,
	IReviewRepository reviewRepository) : Controller
{
	/// <summary>
	/// Add review to a trainer
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	[Authorize(Roles = "Customer")]
	[HttpPost("add")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> AddReview([FromBody] AddReviewRequestDto body)
	{
		var (email, phoneNumber) = userService.GetCredentials();
		if (email == null && phoneNumber == null)
			return BadRequest(new ResponseError("Can't read credentials from jwt"));

		var user = await userManager.FindByCredentialsAsync(email, phoneNumber);
		if (user == null)
			return BadRequest(new ResponseError("There is no use with these credentials"));

		var customer = user.Customer;
		if (customer == null)
			return BadRequest(new ResponseError("There is no customer associated with this user"));

		var trainer = await trainerRepository.GetByIdAsync(body.TrainerId);
		if (trainer == null)
			return BadRequest(new ResponseError("There is no trainer with this id"));

		var addingResult =
			await reviewRepository.AddReviewAsync(body.ReviewTitle, body.ReviewText,
				DateOnly.FromDateTime(DateTime.Now), customer, trainer);

		return addingResult
			? Ok()
			: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
	}
}