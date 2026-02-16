using AutoMapper;
using childspace_backend.Data;
using childspace_backend.Models.DTOs;
using childspace_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace childspace_backend.Repositories
{
    public class TrialRequestRepository : ITrialRequestRepository
    {
        private readonly ChildSpaceDbContext _context;
        private readonly IMapper _mapper;

        public TrialRequestRepository(ChildSpaceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TrialRequestDto>> GetAllAsync()
        {
            var requests = await _context.TrialRequests
                .Include(r => r.Center)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TrialRequestDto>>(requests);
        }

        public async Task<TrialRequestDto?> GetByIdAsync(Guid id)
        {
            var request = await _context.TrialRequests
                .Include(r => r.Center)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return null;

            return _mapper.Map<TrialRequestDto>(request);
        }

        public async Task<TrialRequestDto> CreateAsync(TrialRequestCreateDto dto)
        {
            var request = new TrialRequest
            {
                Id = Guid.NewGuid(),
                CenterId = dto.CenterId,
                ParentName = dto.ParentName,
                Phone = dto.Phone,
                Email = dto.Email,
                ChildName = dto.ChildName,
                ChildAge = dto.ChildAge,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.TrialRequests.Add(request);
            await _context.SaveChangesAsync();

            return _mapper.Map<TrialRequestDto>(request);
        }

        public async Task<TrialRequestDto?> UpdateAsync(Guid id, TrialRequestUpdateDto dto)
        {
            var request = await _context.TrialRequests.FindAsync(id);

            if (request == null)
                return null;

            request.ParentName = dto.ParentName;
            request.Phone = dto.Phone;
            request.Email = dto.Email;
            request.ChildName = dto.ChildName;
            request.ChildAge = dto.ChildAge;
            request.Comment = dto.Comment;

            await _context.SaveChangesAsync();

            return _mapper.Map<TrialRequestDto>(request);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var request = await _context.TrialRequests.FindAsync(id);

            if (request == null)
                return false;

            _context.TrialRequests.Remove(request);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
