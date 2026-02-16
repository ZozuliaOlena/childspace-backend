using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface IGroupRepository
    {
        Task<IEnumerable<GroupDto>> GetAllAsync();

        Task<GroupDto?> GetByIdAsync(Guid id);

        Task<GroupDto> CreateAsync(GroupCreateDto dto);

        Task<GroupDto?> UpdateAsync(Guid id, GroupUpdateDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}
