using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface ICenterRepository
    {
        Task<IEnumerable<CenterDto>> GetAllAsync(Guid? centerId = null);
        Task<CenterDto> GetByIdAsync(Guid id);
        Task<CenterDto> CreateAsync(CenterCreateDto dto);
        Task<CenterDto> UpdateAsync(Guid id, CenterUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
