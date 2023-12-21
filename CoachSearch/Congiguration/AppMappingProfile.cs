using AutoMapper;
using CoachSearch.Data.Entities;
using CoachSearch.Models.Dto;

namespace CoachSearch.Congiguration;

public class AppMappingProfile: Profile
{
	public AppMappingProfile()
	{
		CreateMap<TrainerProfileRequestDto, Trainer>();
	}
}