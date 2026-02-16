using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface IMaterialRepository
    {
        Task<IEnumerable<MaterialDto>> GetAllAsync();

        Task<MaterialDto?> GetByIdAsync(Guid id);

        Task<MaterialDto> CreateAsync(MaterialCreateDto dto);

        Task<MaterialDto?> UpdateAsync(Guid id, MaterialUpdateDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}
