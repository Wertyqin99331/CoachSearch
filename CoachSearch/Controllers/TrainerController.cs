using CoachSearch.Data.Entities;
using CoachSearch.Extensions;
using CoachSearch.Models;
using CoachSearch.Models.Dto;
using CoachSearch.Models.Dto.ProfileDto;
using CoachSearch.Models.Dto.Review;
using CoachSearch.Models.Enums;
using CoachSearch.Repositories.Trainer;
using CoachSearch.Repositories.TrainingProgram;
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
	UserManager<ApplicationUser> userManager) : Controller
{
	/// <summary>
	/// Get all trainers info
	/// </summary>
	/// <param name="city">City of the trainer</param>
	/// <returns></returns>
	[HttpGet]
	[ProducesResponseType<List<AllTrainersResponseDto>>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetTrainers([FromQuery] string? city)
	{
		var trainerQuery = trainerRepository.GetAllByQuery();

		/*trainerQuery = trainerQuery.Where(t =>
			t.FirstName != null && t.MiddleName != null
			                    && t.LastName != null && t.City != null && t.Specialization != null);*/

		if (!string.IsNullOrWhiteSpace(city))
			trainerQuery = trainerQuery.Where(t => t.City!.ToLower() == city.ToLower());

		var result = await trainerQuery
			.Select(t => new AllTrainersResponseDto()
			{
				TrainerId = t.TrainerId,
				FirstName = t.FirstName,
				MiddleName = t.MiddleName,
				LastName = t.LastName,
				City = t.City,
				AvatarUrl = t.AvatarUrl,
				Specialization = t.Specialization
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
			FirstName = trainer.FirstName,
			MiddleName = trainer.MiddleName,
			LastName = trainer.LastName,
			Info = trainer.Info,
			Specialization = trainer.Specialization,
			AvatarUrl = trainer.AvatarUrl,
			TelegramLink = trainer.TelegramLink,
			City = trainer.City,
			InstagramLink = trainer.InstagramLink,
			TrainingPrograms = await trainingProgramRepository
				.GetAllByTrainerIdToDto(trainer.TrainerId)
				.ToListAsync(),
			Reviews = trainer.Reviews.Select(r => new ReviewDto()
			{
				ReviewDate = r.ReviewDate,
				ReviewText = r.ReviewText,
				CustomerName = r.Customer.FirstName,
				ProgramName = r.ReviewTitle
			}).ToList()
		};

		return Ok(result);
	}

	/*[Authorize(Roles = "Trainer")]
	[HttpPost("programs")]
	public async Task<IActionResult> AddTrainingPrograms([FromBody] AddTrainingProgramsRequestDto body)
	{
		var (email, phoneNumber) = userService.GetCredentials();
		if (email == null && phoneNumber == null)
			return BadRequest(new ResponseError("No credentials in jwt"));

		var user = await userManager.FindByCredentialsAsync(email, phoneNumber);
		if (user == null)
			return BadRequest(new ResponseError("Can't find user with these credentials"));

		var trainerInfo = user.Trainer;
		if (trainerInfo == null)
			return BadRequest(new ResponseError("There is no trainer associated with this user"));
		
		var result = await trainingProgramRepository.AddTrainingProgramsByTrainerIdAsync(trainerInfo.TrainerId, body.TrainingPrograms.Select(td => new TrainingProgram()
		{
			Trainer = trainerInfo,
			TrainingProgramName = td.TrainingProgramName,
			TrainingProgramPrice = td.TrainingProgramPrice
		}));
		return result
			? Ok()
			: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
	}*/
	
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
				FirstName = trainerInfo.FirstName,
				MiddleName = trainerInfo.MiddleName,
				LastName = trainerInfo.LastName,
				City = trainerInfo.City,
				Info = trainerInfo.Info,
				Specialization = trainerInfo.Specialization,
				AvatarUrl = trainerInfo.AvatarUrl,
				TelegramLink = trainerInfo.TelegramLink,
				InstagramLink = trainerInfo.InstagramLink,
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
	public async Task<IActionResult> UpdateTrainerProfile([FromBody] TrainerProfileRequestDto body)
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
			var newTrainerInfo = new Trainer()
			{
				FirstName = body.FirstName,
				MiddleName = body.MiddleName,
				LastName = body.LastName,
				City = body.City,
				Specialization = body.Specialization,
				UserInfo = user,
				InstagramLink = body.InstagramLink,
				TelegramLink = body.TelegramLink,
				AvatarUrl = null,
				Info = body.Info,
				TrainingPrograms = body.TrainingPrograms.Select(t => new TrainingProgram()
				{
					TrainingProgramName = t.TrainingProgramName,
					TrainingProgramPrice = t.TrainingProgramPrice
				}).ToList()
			};

			var result = await trainerRepository.AddAsync(newTrainerInfo);
			return result
				? Ok()
				: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
		}
		else
		{
			var result = await trainerRepository.UpdateAsync(trainerInfo.TrainerId, body);
			return result
				? Ok()
				: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
		}
	}
}