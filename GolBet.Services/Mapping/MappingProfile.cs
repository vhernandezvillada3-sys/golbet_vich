// GolBet.Services/Mapping/MappingProfile.cs
using AutoMapper;
using GolBet.Entities;
using GolBet.Services.DTOs;

namespace GolBet.Services.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Flattening by convention:
        CreateMap<Match, MatchDto>();

        // Detail map: TotalBets cannot be resolved by convention
        CreateMap<Match, MatchDetailDto>()
            .ForMember(dto => dto.TotalBets,
                       options => options.MapFrom(match => match.Bets.Count));

        // Nuevos mapeos para el Módulo 6
        CreateMap<Team, TeamDto>();

        CreateMap<TeamFormDto, Team>().ReverseMap();

        CreateMap<MatchFormDto, Match>().ReverseMap();
    }
}