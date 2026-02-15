using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface IChildRepository
    {
        Task<ChildDto?> GetByIdAsync(Guid id);

        Task<IEnumerable<ChildDto>> GetAllAsync();

        Task<IEnumerable<ChildDto>> GetByParentIdAsync(Guid parentId);

        Task<ChildDto?> CreateAsync(CreateChildDto dto);

        Task<ChildDto?> UpdateAsync(Guid id, UpdateChildDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}
