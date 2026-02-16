using AutoMapper;
using childspace_backend.Data;
using childspace_backend.Models;
using childspace_backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace childspace_backend.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ChildSpaceDbContext _context;
        private readonly IMapper _mapper;

        public GroupRepository(ChildSpaceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GroupDto>> GetAllAsync()
        {
            var groups = await _context.Groups
                .Include(g => g.Center)
                .Include(g => g.Teacher)
                .Include(g => g.GroupChildren)
                .Include(g => g.Schedules)
                .Include(g => g.Materials)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GroupDto>>(groups);
        }

        public async Task<GroupDto?> GetByIdAsync(Guid id)
        {
            var group = await _context.Groups
                .Include(g => g.Center)
                .Include(g => g.Teacher)
                .Include(g => g.GroupChildren)
                .Include(g => g.Schedules)
                .Include(g => g.Materials)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
                return null;

            return _mapper.Map<GroupDto>(group);
        }

        public async Task<GroupDto> CreateAsync(GroupCreateDto dto)
        {
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                CenterId = dto.CenterId,
                TeacherId = dto.TeacherId,
                Description = dto.Description
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return _mapper.Map<GroupDto>(group);
        }

        public async Task<GroupDto?> UpdateAsync(Guid id, GroupUpdateDto dto)
        {
            var group = await _context.Groups.FindAsync(id);

            if (group == null)
                return null;

            group.Name = dto.Name;
            group.TeacherId = dto.TeacherId;
            group.Description = dto.Description;

            await _context.SaveChangesAsync();

            return _mapper.Map<GroupDto>(group);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var group = await _context.Groups.FindAsync(id);

            if (group == null)
                return false;

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
