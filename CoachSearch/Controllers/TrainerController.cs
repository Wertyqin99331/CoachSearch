using CoachSearch.Data.Entities;
using CoachSearch.Extensions;
using CoachSearch.Models;
using CoachSearch.Models.Dto;
using CoachSearch.Models.Dto.ProfileDto;
using CoachSearch.Models.Dto.Review;
using CoachSearch.Models.Dto.Trainer;
using CoachSearch.Models.Enums;
using CoachSearch.Repositories.Trainer;
using CoachSearch.Repositories.TrainingProgram;
using CoachSearch.Services.FileUploadService;
using CoachSearch.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoachSearch.Controllers;

[ApiController]
[Route("api/trainer")]
public class TrainerController(
	ITrainerRepository trainerRepository,
	ITrainingProgramRepository trainingProgramRepository,
	IUserService userService,
	UserManager<ApplicationUser> userManager,
	IFileUploadService fileUploadService): Controller
{
	/// <summary>
	/// Get all trainers info
	/// </summary>
	/// <param name="city">City of the trainer</param>
	/// <returns></returns>
	[HttpGet]
	[ProducesResponseType<List<AllTrainersResponseDto>>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetTrainers()
	{
		var trainerQuery = trainerRepository.GetAllByQuery();

		/*trainerQuery = trainerQuery.Where(t =>
			t.FirstName != null && t.MiddleName != null
			                    && t.LastName != null && t.City != null && t.Specialization != null);*/


		var (email, phoneNumber) = userService.GetCredentials();
		var currentUser = userManager.FindByCredentialsAsync(email, phoneNumber);
		
		var result = await trainerQuery
			.Select(t => new AllTrainersResponseDto()
			{
				TrainerId = t.TrainerId,
				FullName = t.FullName,
				AvatarUrl = fileUploadService.GetAvatarUrl(Request, t.AvatarFileName),
				Specialization = t.Specialization,
				LikesCount = t.Likes.Count,
				IsLiked = t.Likes.Any(l => l.CustomerId == currentUser.Id)
			})
			.ToListAsync();
		return Ok(result);
	}

	/// <summary>
	/// Get a trainer info by id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	[HttpGet("{id:long}")]
	[ProducesResponseType<TrainerByIdResponseDto>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetTrainerById(long id)
	{
		var trainer = await trainerRepository.GetByIdAsync(id);

		if (trainer == null)
			return BadRequest(new ResponseError("There is no trainer with this id"));

		var result = new TrainerByIdResponseDto()
		{
			TrainerId = trainer.TrainerId,
			Email = trainer.UserInfo.Email,
			PhoneNumber = trainer.UserInfo.PhoneNumber,
			Address = trainer.Address,
			FullName = trainer.FullName,
			Info = trainer.Info,
			Specialization = trainer.Specialization,
			AvatarUrl = fileUploadService.GetAvatarUrl(Request, trainer.AvatarFileName),
			TelegramLink = trainer.TelegramLink,
			VkLink = trainer.VkLink,
			TrainingPrograms = await trainingProgramRepository
				.GetAllByTrainerIdToDto(trainer.TrainerId)
				.ToListAsync(),
			Reviews = trainer.Reviews.Select(r => new ReviewDto()
			{
				ReviewDate = r.ReviewDate,
				ReviewText = r.ReviewText,
				CustomerName = r.Customer.FullName,
				ProgramName = r.ReviewTitle
			}).ToList(),
			LikesCount = trainer.Likes.Count
		};

		return Ok(result);
	}
	
	/// <summary>
	/// Get a trainer profile
	/// </summary>
	/// <returns></returns>
	[Authorize(Roles = "Trainer")]
	[HttpGet("profile")]
	[ProducesResponseType<TrainerProfileResponseDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetTrainerProfile()
	{
		var (email, phoneNumber) = userService.GetCredentials();

		if (email is null && phoneNumber is null)
			return BadRequest(new ResponseError("Can't read credentials from jwt"));

		var user = await userManager.FindByCredentialsAsync(email, phoneNumber);
		
		if (user is null)
			return BadRequest("There is no user with these credentials");

		var userRole = userService.GetUserRole();

		if (userRole == UserRole.Trainer)
		{
			var trainerInfo = user.Trainer;

			if (trainerInfo == null)
				return BadRequest(new ResponseError("There is no trainer associated with user"));

			return Ok(new TrainerProfileResponseDto()
			{
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				FullName = trainerInfo.FullName,
				Address = trainerInfo.Address,
				Info = trainerInfo.Info,
				Specialization = trainerInfo.Specialization,
				AvatarUrl = fileUploadService.GetAvatarUrl(Request, trainerInfo.AvatarFileName),
				TelegramLink = trainerInfo.TelegramLink,
				VkLink = trainerInfo.VkLink,
				TrainingPrograms = await trainingProgramRepository
					.GetAllByTrainerIdToDto(trainerInfo.TrainerId)
					.ToListAsync()
			});
		}

		return BadRequest(new ResponseError("Can't read user role from jwt"));
	}

	/// <summary>
	/// Update a trainer profile
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	[Authorize(Roles = "Trainer")]
	[HttpPost("profile")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UpdateTrainerProfile([FromForm] TrainerProfileRequestDto body)
	{
		var (email, phoneNumber) = userService.GetCredentials();
		if (email == null && phoneNumber == null)
			return BadRequest(new ResponseError("Can't read credentials from jwt"));

		var user = await userManager.FindByCredentialsAsync(email, phoneNumber);
		if (user == null)
			return BadRequest(new ResponseError("There is no user with these credentials"));

		var trainerInfo = user.Trainer;
		if (trainerInfo == null)
		{
			var fileName = body.Avatar != null
				? await fileUploadService.UploadFileAsync(body.Avatar)
				: null;
			
			var newTrainerInfo = new Trainer()
			{
				FullName = body.FullName,
				Specialization = body.Specialization,
				UserInfo = user,
				VkLink = body.VkLink,
				TelegramLink = body.TelegramLink, 
				AvatarFileName = fileName,
				Info = body.Info,
				Address = body.Address
			};

			var result = await trainerRepository.AddAsync(newTrainerInfo);
			
			return result
				? NoContent()
				: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
		}
		else
		{
			var result = await trainerRepository.UpdateAsync(trainerInfo.TrainerId, body);
			return result
				? NoContent()
				: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
		}
	}
	
	/// <summary>
	/// Update training programs to the trainer profile
	/// </summary>
	/// <param name="body"></param>
	/// <returns></returns>
	[Authorize(Roles = "Trainer")]
	[HttpPost("profile/trainingPrograms")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType<ResponseError>(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UpdateTrainingPrograms([FromBody] UpdateTrainingProgramsRequestDto body)
	{
		var (email, phoneNumber) = userService.GetCredentials();
		if (email == null && phoneNumber == null)
			return BadRequest(new ResponseError("Can't read credentials from jwt"));

		var user = await userManager.FindByCredentialsAsync(email, phoneNumber);
		if (user == null)
			return BadRequest(new ResponseError("There is no user with these credentials"));

		var trainerInfo = user.Trainer;
		if (trainerInfo == null)
			return BadRequest(new ResponseError("There is no trainer profile associated with this user"));

		var updateResult = await trainerRepository.UpdateTrainingProgramsAsync(trainerInfo.TrainerId, body.TrainingPrograms);
		return updateResult
			? NoContent()
			: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something went wrong"));
	}

	/// <summary>
	/// Get all trainer addresses
	/// </summary>
	/// <returns></returns>
	[HttpGet("addresses")]
	public async Task<IActionResult> GetAllAddresses()
	{
		var allAddresses = await trainerRepository.GetAllAddresses();
		return Ok(new AllTrainerAddressesResponseDto()
		{
			Addresses = allAddresses
		});
	}

	/*[NonAction]
	private static string? GetAvatarUrl(HttpRequest request, string? fileName) => fileName == null
		? null
		: $"{request.Scheme}://{request.Host}{request.PathBase}/Images/{fileName}";*/
}