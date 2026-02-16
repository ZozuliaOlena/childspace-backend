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
            var chats = await _context.Chats
                .Include(c => c.UserChats)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ChatDto>>(chats);
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
    }
}
