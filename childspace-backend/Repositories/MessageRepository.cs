using AutoMapper;
using childspace_backend.Data;
using childspace_backend.Models.DTOs;
using childspace_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace childspace_backend.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChildSpaceDbContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(ChildSpaceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MessageDto>> GetAllAsync()
        {
            var messages = await _context.Messages
                .Include(m => m.UserChat)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<MessageDto?> GetByIdAsync(Guid id)
        {
            var message = await _context.Messages
                .Include(m => m.UserChat)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
                return null;

            return _mapper.Map<MessageDto>(message);
        }

        public async Task<MessageDto> CreateAsync(MessageCreateDto dto)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                UserChatId = dto.UserChatId,
                Content = dto.Content
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return _mapper.Map<MessageDto>(message);
        }

        public async Task<MessageDto?> UpdateAsync(Guid id, MessageUpdateDto dto)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
                return null;

            message.Content = dto.Content;

            await _context.SaveChangesAsync();

            return _mapper.Map<MessageDto>(message);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
                return false;

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
