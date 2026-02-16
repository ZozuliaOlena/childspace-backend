using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface IUserChatRepository
    {
        Task<IEnumerable<UserChatDto>> GetAllAsync();

        Task<UserChatDto?> GetByIdAsync(Guid id);

        Task<UserChatDto> CreateAsync(UserChatCreateDto dto);

        Task<UserChatDto?> UpdateAsync(Guid id, UserChatUpdateDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}
