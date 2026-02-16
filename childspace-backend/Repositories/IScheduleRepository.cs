using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface IScheduleRepository
    {
        Task<IEnumerable<ScheduleDto>> GetAllAsync();

        Task<ScheduleDto?> GetByIdAsync(Guid id);

        Task<ScheduleDto> CreateAsync(ScheduleCreateDto dto);

        Task<ScheduleDto?> UpdateAsync(Guid id, ScheduleUpdateDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}
