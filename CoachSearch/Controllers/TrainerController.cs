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
public class TrainerController: Controller
{

	public TrainerController(ITrainerRepository trainerRepository, ITrainingProgramRepository trainingProgramRepository, IUserService userService, UserManager<ApplicationUser> userManager, IFileUploadService fileUploadService)
	{
		_trainerRepository = trainerRepository;
		_trainingProgramRepository = trainingProgramRepository;
		_userService = userService;
		_userManager = userManager;
		_fileUploadService = fileUploadService;
	}

	private readonly ITrainerRepository _trainerRepository;
	private readonly ITrainingProgramRepository _trainingProgramRepository;
	private readonly IUserService _userService;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IFileUploadService _fileUploadService;
	
	/// <summary>
	/// Get all trainers info
	/// </summary>
	/// <param name="city">City of the trainer</param>
	/// <returns></returns>
	[HttpGet]
	[ProducesResponseType(typeof(List<AllTrainersResponseDto>),StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetTrainers()
	{
		var trainerQuery = _trainerRepository.GetAllByQuery();

		/*trainerQuery = trainerQuery.Where(t =>
			t.FirstName != null && t.MiddleName != null
			                    && t.LastName != null && t.City != null && t.Specialization != null);*/


		var (email, phoneNumber) = _userService.GetCredentials();
		var currentUser = email == null && phoneNumber == null
			? null
			: await _userManager.FindByCredentialsAsync(email, phoneNumber);
		
		
		var result = await trainerQuery
			.Select(t => new AllTrainersResponseDto()
			{
				TrainerId = t.TrainerId,
				FullName = t.FullName,
				Address = t.Address,
				AvatarUrl = _fileUploadService.GetAvatarUrl(Request, t.AvatarFileName),
				Specialization = t.Specialization,
				LikesCount = t.Likes.Count,
				IsLiked = currentUser != null 
				          && currentUser.Customer != null 
				          && t.Likes.Any(l => l.CustomerId == currentUser.Customer.CustomerId)
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
	[ProducesResponseType(typeof(TrainerByIdResponseDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetTrainerById(long id)
	{
		var trainer = await _trainerRepository.GetByIdAsync(id);

		if (trainer == null)
			return BadRequest(new ResponseError("There is no trainer with this id"));
		
		var (email, phoneNumber) = _userService.GetCredentials();
		var currentUser = email == null && phoneNumber == null
			? null
			: await _userManager.FindByCredentialsAsync(email, phoneNumber);

		var result = new TrainerByIdResponseDto()
		{
			TrainerId = trainer.TrainerId,
			Email = trainer.UserInfo.Email,
			PhoneNumber = trainer.UserInfo.PhoneNumber,
			Address = trainer.Address,
			FullName = trainer.FullName,
			Info = trainer.Info,
			Specialization = trainer.Specialization,
			AvatarUrl = _fileUploadService.GetAvatarUrl(Request, trainer.AvatarFileName),
			TelegramLink = trainer.TelegramLink,
			VkLink = trainer.VkLink,
			TrainingPrograms = await _trainingProgramRepository
				.GetAllByTrainerIdToDto(trainer.TrainerId)
				.ToListAsync(),
			Reviews = trainer.Reviews.Select(r => new ReviewDto()
			{
				ReviewDate = r.ReviewDate,
				ReviewText = r.ReviewText,
				CustomerName = r.Customer.FullName,
				AvatarUrl = _fileUploadService.GetAvatarUrl(Request, r.Customer.AvatarFileName)
			}).ToList(),
			LikesCount = trainer.Likes.Count,
			IsLiked = currentUser != null && currentUser.Customer != null && trainer.Likes.Any(l => l.CustomerId == currentUser.Customer.CustomerId)
		};

		return Ok(result);
	}
	
	/// <summary>
	/// Get a trainer profile
	/// </summary>
	/// <returns></returns>
	[Authorize(Roles = "Trainer")]
	[HttpGet("profile")]
	[ProducesResponseType(typeof(TrainerByIdResponseDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> 
		GetTrainerProfile()
	{
		var (email, phoneNumber) = _userService.GetCredentials();

		if (email is null && phoneNumber is null)
			return BadRequest(new ResponseError("Can't read credentials from jwt"));

		var user = await _userManager.FindByCredentialsAsync(email, phoneNumber);
		
		if (user is null)
			return BadRequest("There is no user with these credentials");

		var userRole = _userService.GetUserRole();

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
				AvatarUrl = _fileUploadService.GetAvatarUrl(Request, trainerInfo.AvatarFileName),
				TelegramLink = trainerInfo.TelegramLink,
				VkLink = trainerInfo.VkLink,
				TrainingPrograms = await _trainingProgramRepository
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
	[ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UpdateTrainerProfile([FromForm] TrainerProfileRequestDto body)
	{
		var (email, phoneNumber) = _userService.GetCredentials();
		if (email == null && phoneNumber == null)
			return BadRequest(new ResponseError("Can't read credentials from jwt"));

		var user = await _userManager.FindByCredentialsAsync(email, phoneNumber);
		if (user == null)
			return BadRequest(new ResponseError("There is no user with these credentials"));

		var trainerInfo = user.Trainer;
		if (trainerInfo == null)
		{
			var fileName = body.Avatar != null
				? await _fileUploadService.UploadFileAsync(body.Avatar)
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

			var result = await _trainerRepository.AddAsync(newTrainerInfo);
			
			return result
				? NoContent()
				: StatusCode(StatusCodes.Status500InternalServerError, new ResponseError("Something goes wrong"));
		}
		else
		{
			var result = await _trainerRepository.UpdateAsync(trainerInfo.TrainerId, body);
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
	[ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ResponseError), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UpdateTrainingPrograms([FromBody] UpdateTrainingProgramsRequestDto body)
	{
		var (email, phoneNumber) = _userService.GetCredentials();
		if (email == null && phoneNumber == null)
			return BadRequest(new ResponseError("Can't read credentials from jwt"));

		var user = await _userManager.FindByCredentialsAsync(email, phoneNumber);
		if (user == null)
			return BadRequest(new ResponseError("There is no user with these credentials"));

		var trainerInfo = user.Trainer;
		if (trainerInfo == null)
			return BadRequest(new ResponseError("There is no trainer profile associated with this user"));

		var updateResult = await _trainerRepository.UpdateTrainingProgramsAsync(trainerInfo.TrainerId, body.TrainingPrograms);
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
		var allAddresses = await _trainerRepository.GetAllAddresses();
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