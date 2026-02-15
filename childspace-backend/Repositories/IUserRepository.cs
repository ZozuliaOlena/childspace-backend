using childspace_backend.Models;
using childspace_backend.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace childspace_backend.Repositories
{
    public interface IUserRepository
    {
        Task<UserDto> GetByIdAsync(Guid id);
        Task<IEnumerable<UserDto>> GetAllAsync();

        Task<UserDto> CreateAsync(UserDto dto, string password);

        Task<UserDto> UpdateAsync(Guid id, UserUpdateRequest dto);

        Task<bool> DeleteAsync(Guid id);

        Task<IList<string>> GetAllRolesAsync();

        Task<IList<string>> GetUserRolesAsync(User user);

        Task<IdentityResult> AddToRolesAsync(User user, IEnumerable<string> roles);

        Task<IdentityResult> RemoveFromRolesAsync(User user, IEnumerable<string> roles);
    }
}
