using AutoMapper;
using childspace_backend.Data;
using childspace_backend.Models.DTOs;
using childspace_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace childspace_backend.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChildSpaceDbContext _context;
        private readonly IMapper _mapper;

        public ChatRepository(ChildSpaceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ChatDto>> GetAllAsync()
        {
            var chatsQuery = await _context.Chats
                .Select(chat => new
                {
                    Chat = chat,
                    ParticipantCount = chat.UserChats.Count(),

                    LastMessage = _context.Messages
                        .Where(m => m.UserChat.ChatId == chat.Id)
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => new ChatMessageResponseDto
                        {
                            Id = m.Id,
                            Content = m.Content,
                            CreatedAt = m.CreatedAt,
                            SenderId = m.UserChat.UserId,
                            SenderName = m.UserChat.User.FirstName + " " + m.UserChat.User.LastName
                        })
                        .FirstOrDefault()
                })
                .ToListAsync();

            var chatDtos = chatsQuery.Select(x => new ChatDto
            {
                Id = x.Chat.Id,
                Name = x.Chat.Name,
                CreatedAt = x.Chat.CreatedAt,
                ParticipantsCount = x.ParticipantCount,
                LastMessage = x.LastMessage
            })
            .OrderByDescending(c => c.LastMessage?.CreatedAt ?? c.CreatedAt)
            .ToList();

            return chatDtos;
        }

        public async Task<ChatDto?> GetByIdAsync(Guid id)
        {
            var chat = await _context.Chats
                .Include(c => c.UserChats)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chat == null)
                return null;

            return _mapper.Map<ChatDto>(chat);
        }

        public async Task<ChatDto> CreateAsync(ChatCreateDto dto)
        {
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Name = dto.Name
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return _mapper.Map<ChatDto>(chat);
        }

        public async Task<ChatDto?> UpdateAsync(Guid id, ChatUpdateDto dto)
        {
            var chat = await _context.Chats.FindAsync(id);

            if (chat == null)
                return null;

            chat.Name = dto.Name;

            await _context.SaveChangesAsync();

            return _mapper.Map<ChatDto>(chat);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var chat = await _context.Chats.FindAsync(id);

            if (chat == null)
                return false;

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<UserDto>> GetChatParticipantsAsync(Guid chatId)
        {
            var users = await _context.UserChats
                .Where(uc => uc.ChatId == chatId)
                .Select(uc => uc.User)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
    }
}
