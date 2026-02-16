using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface IAttendanceRepository
    {
        Task<IEnumerable<AttendanceDto>> GetAllAsync();

        Task<AttendanceDto?> GetByIdAsync(Guid id);

        Task<AttendanceDto> CreateAsync(AttendanceCreateDto dto);

        Task<AttendanceDto?> UpdateAsync(Guid id, AttendanceUpdateDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}
