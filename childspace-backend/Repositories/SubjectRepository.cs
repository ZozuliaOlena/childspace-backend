using AutoMapper;
using childspace_backend.Models.DTOs;
using childspace_backend.Models;
using childspace_backend.Data;
using Microsoft.EntityFrameworkCore;

namespace childspace_backend.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ChildSpaceDbContext _context;
        private readonly IMapper _mapper;

        public SubjectRepository(ChildSpaceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubjectDto>> GetAllAsync(Guid? centerId = null)
        {
            var query = _context.Subjects.AsQueryable();

            if (centerId.HasValue)
            {
                query = query.Where(s => s.CenterId == centerId.Value);
            }

            var subjects = await query.ToListAsync();

            return _mapper.Map<IEnumerable<SubjectDto>>(subjects);
        }

        public async Task<SubjectDto?> GetByIdAsync(Guid id)
        {
            var subject = await _context.Subjects
                .Include(s => s.Center)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
                return null;

            return _mapper.Map<SubjectDto>(subject);
        }

        public async Task<SubjectDto> CreateAsync(SubjectCreateDto dto, string? photoUrl)
        {
            var subject = new Subject
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                CenterId = dto.CenterId,
                PhotoUrl = photoUrl 
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return _mapper.Map<SubjectDto>(subject);
        }

        public async Task<SubjectDto?> UpdateAsync(Guid id, SubjectUpdateDto dto, string? photoUrl)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return null;

            subject.Name = dto.Name;
            subject.Description = dto.Description;
            subject.PhotoUrl = photoUrl; 

            await _context.SaveChangesAsync();

            return _mapper.Map<SubjectDto>(subject);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
                return false;

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<SubjectDto>> GetSubjectsByTeacherAsync(Guid teacherId)
        {
            var teacher = await _context.Users
                .Include(u => u.Center)
                .FirstOrDefaultAsync(u => u.Id == teacherId);

            if (teacher == null || teacher.Center == null)
                return Enumerable.Empty<SubjectDto>();

            return await _context.Subjects
                .Where(s => s.CenterId == teacher.Center.Id)
                .Select(s => new SubjectDto { Id = s.Id, Name = s.Name })
                .ToListAsync();
        }
    }
}
