using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<SubjectDto>> GetAllAsync();

        Task<SubjectDto?> GetByIdAsync(Guid id);

        Task<SubjectDto> CreateAsync(SubjectCreateDto dto);

        Task<SubjectDto?> UpdateAsync(Guid id, SubjectUpdateDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}
