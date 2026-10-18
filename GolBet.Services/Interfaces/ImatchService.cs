using GolBet.Entities.Enums;
using GolBet.Services.DTOs;

namespace GolBet.Services.Interfaces;

public interface IMatchService
{
    Task<IEnumerable<MatchDto>> GetBoardAsync(MatchStatus? status = null);
    Task<MatchDetailDto?> GetDetailAsync(int id);

    // Nuevos métodos del Módulo 6
    Task<MatchFormDto?> GetForEditAsync(int id);
    Task CreateAsync(MatchFormDto dto);
    Task UpdateAsync(MatchFormDto dto);
    Task DeactivateAsync(int id);
}