using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface IChatRepository
    {
        Task<IEnumerable<ChatDto>> GetAllAsync();

        Task<ChatDto?> GetByIdAsync(Guid id);

        Task<ChatDto> CreateAsync(ChatCreateDto dto);

        Task<ChatDto?> UpdateAsync(Guid id, ChatUpdateDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}
