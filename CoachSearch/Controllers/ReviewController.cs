using CoachSearch.Data.Entities;
using CoachSearch.Extensions;
using CoachSearch.Models;
using CoachSearch.Models.Dto.Review;
using CoachSearch.Repositories.Review;
using CoachSearch.Repositories.Trainer;
using CoachSearch.Services.FileUploadService;
using CoachSearch.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoachSearch.Controllers;

[ApiController]
[Route("review")]
public class ReviewController : Controller
{
	public ReviewController(
		IUserService userService,
		UserManager<ApplicationUser> userManager,
		ITrainerRepository trainerRepository,
		IReviewRepository reviewRepository,
		IFileUploadService fileUploadService)
	{
		this._userService = userService;
		this._userManager = userManager;
		this._trainerRepository = trainerRepository;
		this._reviewRepository = reviewRepository;
		this._fileUploadService = fileUploadService;
	}
	
	private readonly IUserService _userService;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly ITrainerRepository _trainerRepository;
	private readonly IReviewRepository _reviewRepository;
	private readonly IFileUploadService _fileUploadService;
	/// <summary>
	/// Add review to a trainer
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	[Authorize(Roles = "Customer")]
	[HttpPost("add")]
	[ProducesResponseType(typeof(Review), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ResponseError), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> AddReview([FromBody] AddReviewRequestDto body)
	{
		var (email, phoneNumber) = this._userService.GetCredentials();
		if (email == null && phoneNumber == null)
			return BadRequest(new ResponseError("Can't read credentials from jwt"));

		var user = await this._userManager.FindByCredentialsAsync(email, phoneNumber);
		if (user == null)
			return BadRequest(new ResponseError("There is no use with these credentials"));

		var customer = user.Customer;
		if (customer == null)
			return BadRequest(new ResponseError("There is no customer associated with this user"));

		var trainer = await this._trainerRepository.GetByIdAsync(body.TrainerId);
		if (trainer == null)
			return BadRequest(new ResponseError("There is no trainer with this id"));

		var addingResult =
			await this._reviewRepository.AddReviewAsync(body.ReviewText,
				DateTime.Now, customer, trainer);

		return addingResult != null
			? Ok(new ReviewDto()
			{
				CustomerName = addingResult.Customer.FullName,
				ReviewDate = addingResult.ReviewDate,
				ReviewText = addingResult.ReviewText,
				AvatarUrl = this._fileUploadService.GetAvatarUrl(Request, addingResult.Customer.AvatarFileName)
			})
			: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
	}
}