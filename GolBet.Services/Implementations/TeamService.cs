// GolBet.Services/Implementations/TeamService.cs
using AutoMapper;
using GolBet.Entities;
using GolBet.Repositories.Interfaces;
using GolBet.Services.DTOs;
using GolBet.Services.Interfaces;

namespace GolBet.Services.Implementations;

public class TeamService : ITeamService
{
    private readonly IGenericRepository<Team> _teamRepository;
    private readonly IMapper _mapper;

    public TeamService(IGenericRepository<Team> teamRepository, IMapper mapper)
    {
        _teamRepository = teamRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TeamDto>> GetAllAsync()
    {
        var teams = await _teamRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<TeamDto>>(teams);
    }

    public async Task<TeamFormDto?> GetForEditAsync(int id)
    {
        var team = await _teamRepository.GetByIdAsync(id);
        return team is null ? null : _mapper.Map<TeamFormDto>(team);
    }

    public async Task CreateAsync(TeamFormDto dto)
    {
        var team = _mapper.Map<Team>(dto);
        await _teamRepository.AddAsync(team);
    }

    public async Task UpdateAsync(TeamFormDto dto)
    {
        var team = await _teamRepository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Team {dto.Id} not found.");

        _mapper.Map(dto, team); // Aplica los valores del DTO sobre la entidad
        await _teamRepository.UpdateAsync(team);
    }

    public async Task DeactivateAsync(int id)
    {
        await _teamRepository.DeactivateAsync(id);
    }
}