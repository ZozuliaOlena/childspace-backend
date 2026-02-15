using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface IUserRepository
    {
        Task<UserDto> GetByIdAsync(Guid id);
        Task<IEnumerable<UserDto>> GetAllAsync();

        Task<UserDto> CreateAsync(UserDto dto, string password);

        Task<UserDto> UpdateAsync(Guid id, UserUpdateRequest dto);

        Task<bool> DeleteAsync(Guid id);
    }
}
