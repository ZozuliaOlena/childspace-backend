using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface IMessageRepository
    {
        Task<IEnumerable<MessageDto>> GetAllAsync();

        Task<MessageDto?> GetByIdAsync(Guid id);

        Task<MessageDto> CreateAsync(MessageCreateDto dto);

        Task<MessageDto?> UpdateAsync(Guid id, MessageUpdateDto dto);

        Task<bool> DeleteAsync(Guid id);

        Task<IEnumerable<ChatMessageResponseDto>> GetMessagesByChatIdAsync(Guid chatId);
        Task<ChatMessageResponseDto> SendMessageAsync(Guid userId, SendMessageDto dto);
    }
}
