using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface IGroupChildRepository
    {
        Task<IEnumerable<GroupChildDto>> GetAllAsync();

        Task<GroupChildDto?> GetByIdAsync(Guid id);

        Task<GroupChildDto> CreateAsync(GroupChildCreateDto dto);

        Task<GroupChildDto?> UpdateAsync(Guid id, GroupChildUpdateDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}
