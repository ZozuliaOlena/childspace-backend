using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface IMaterialRepository
    {
        Task<IEnumerable<MaterialDto>> GetAllAsync();

        Task<MaterialDto?> GetByIdAsync(Guid id);

        Task<MaterialDto> CreateAsync(MaterialCreateDto dto, string fileUrl);

        Task<MaterialDto?> UpdateAsync(Guid id, MaterialUpdateDto dto, string fileUrl);

        Task<bool> DeleteAsync(Guid id);

        Task<IEnumerable<MaterialDto>> GetBySubjectIdAsync(Guid subjectId);
    }
}
