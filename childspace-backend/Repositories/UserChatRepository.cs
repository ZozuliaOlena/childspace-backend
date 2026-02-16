using AutoMapper;
using childspace_backend.Data;
using childspace_backend.Models.DTOs;
using childspace_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace childspace_backend.Repositories
{
    public class UserChatRepository : IUserChatRepository
    {
        private readonly ChildSpaceDbContext _context;
        private readonly IMapper _mapper;

        public UserChatRepository(ChildSpaceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserChatDto>> GetAllAsync()
        {
            var userChats = await _context.UserChats
                .Include(uc => uc.User)
                .Include(uc => uc.Chat)
                .Include(uc => uc.Messages)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserChatDto>>(userChats);
        }

        public async Task<UserChatDto?> GetByIdAsync(Guid id)
        {
            var userChat = await _context.UserChats
                .Include(uc => uc.User)
                .Include(uc => uc.Chat)
                .Include(uc => uc.Messages)
                .FirstOrDefaultAsync(uc => uc.Id == id);

            if (userChat == null)
                return null;

            return _mapper.Map<UserChatDto>(userChat);
        }

        public async Task<UserChatDto> CreateAsync(UserChatCreateDto dto)
        {
            var userChat = new UserChat
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                ChatId = dto.ChatId
            };

            _context.UserChats.Add(userChat);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserChatDto>(userChat);
        }

        public async Task<UserChatDto?> UpdateAsync(Guid id, UserChatUpdateDto dto)
        {
            var userChat = await _context.UserChats.FindAsync(id);

            if (userChat == null)
                return null;

            userChat.UserId = dto.UserId;
            userChat.ChatId = dto.ChatId;

            await _context.SaveChangesAsync();

            return _mapper.Map<UserChatDto>(userChat);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var userChat = await _context.UserChats.FindAsync(id);

            if (userChat == null)
                return false;

            _context.UserChats.Remove(userChat);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
