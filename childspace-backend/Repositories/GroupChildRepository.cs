using AutoMapper;
using childspace_backend.Data;
using childspace_backend.Models.DTOs;
using childspace_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace childspace_backend.Repositories
{
    public class GroupChildRepository : IGroupChildRepository
    {
        private readonly ChildSpaceDbContext _context;
        private readonly IMapper _mapper;

        public GroupChildRepository(ChildSpaceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GroupChildDto>> GetAllAsync()
        {
            var groupChildren = await _context.GroupChildren
                .Include(gc => gc.Group)
                .Include(gc => gc.Child)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GroupChildDto>>(groupChildren);
        }

        public async Task<GroupChildDto?> GetByIdAsync(Guid id)
        {
            var groupChild = await _context.GroupChildren
                .Include(gc => gc.Group)
                .Include(gc => gc.Child)
                .FirstOrDefaultAsync(gc => gc.Id == id);

            if (groupChild == null)
                return null;

            return _mapper.Map<GroupChildDto>(groupChild);
        }

        public async Task<GroupChildDto> CreateAsync(GroupChildCreateDto dto)
        {
            var groupChild = new GroupChild
            {
                Id = Guid.NewGuid(),
                GroupId = dto.GroupId,
                ChildId = dto.ChildId
            };

            _context.GroupChildren.Add(groupChild);
            await _context.SaveChangesAsync();

            return _mapper.Map<GroupChildDto>(groupChild);
        }

        public async Task<GroupChildDto?> UpdateAsync(Guid id, GroupChildUpdateDto dto)
        {
            var groupChild = await _context.GroupChildren.FindAsync(id);

            if (groupChild == null)
                return null;

            groupChild.GroupId = dto.GroupId;
            groupChild.ChildId = dto.ChildId;

            await _context.SaveChangesAsync();

            return _mapper.Map<GroupChildDto>(groupChild);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var groupChild = await _context.GroupChildren.FindAsync(id);

            if (groupChild == null)
                return false;

            _context.GroupChildren.Remove(groupChild);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
