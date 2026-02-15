using AutoMapper;
using childspace_backend.Data;
using childspace_backend.Models.DTOs;
using childspace_backend.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace childspace_backend.Repositories
{
    public class CenterRepository : ICenterRepository
    {
        private readonly ChildSpaceDbContext _context;
        private readonly IMapper _mapper;

        public CenterRepository(ChildSpaceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CenterDto>> GetAllAsync()
        {
            var centers = await _context.Centers
                .Include(c => c.Users)
                .Include(c => c.Children)
                .Include(c => c.Groups)
                .Include(c => c.TrialRequests)
                .Include(c => c.Subjects)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CenterDto>>(centers);
        }


        public async Task<CenterDto> GetByIdAsync(Guid id)
        {
            var center = await _context.Centers
                .Include(c => c.Users)
                .Include(c => c.Children)
                .Include(c => c.Groups)
                .Include(c => c.TrialRequests)
                .Include(c => c.Subjects)
                .FirstOrDefaultAsync(c => c.Id == id);

            return center == null ? null : _mapper.Map<CenterDto>(center);
        }

        public async Task<CenterDto> CreateAsync(CenterCreateDto dto)
        {
            var center = new Center
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                SubscriptionStatus = dto.SubscriptionStatus
            };

            _context.Centers.Add(center);
            await _context.SaveChangesAsync();

            return _mapper.Map<CenterDto>(center);
        }

        public async Task<CenterDto> UpdateAsync(Guid id, CenterUpdateDto dto)
        {
            var center = await _context.Centers.FindAsync(id);

            if (center == null) return null;

            center.Name = dto.Name;
            center.Address = dto.Address;
            center.Phone = dto.Phone;
            center.Email = dto.Email;
            center.SubscriptionStatus = dto.SubscriptionStatus;

            _context.Centers.Update(center);
            await _context.SaveChangesAsync();

            return _mapper.Map<CenterDto>(center);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var center = await _context.Centers.FindAsync(id);
            if (center == null) return false;

            _context.Centers.Remove(center);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
