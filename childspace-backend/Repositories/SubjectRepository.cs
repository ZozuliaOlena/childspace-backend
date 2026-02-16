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

        public async Task<IEnumerable<SubjectDto>> GetAllAsync()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Center)
                .ToListAsync();

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

        public async Task<SubjectDto> CreateAsync(SubjectCreateDto dto)
        {
            var subject = new Subject
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                CenterId = dto.CenterId
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return _mapper.Map<SubjectDto>(subject);
        }

        public async Task<SubjectDto?> UpdateAsync(Guid id, SubjectUpdateDto dto)
        {
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
                return null;

            subject.Name = dto.Name;
            subject.Description = dto.Description;

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
    }
}
