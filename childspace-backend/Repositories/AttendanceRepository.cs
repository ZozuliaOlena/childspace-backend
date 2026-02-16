using AutoMapper;
using childspace_backend.Data;
using childspace_backend.Models.DTOs;
using childspace_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace childspace_backend.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ChildSpaceDbContext _context;
        private readonly IMapper _mapper;

        public AttendanceRepository(ChildSpaceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AttendanceDto>> GetAllAsync()
        {
            var attendances = await _context.Attendances
                .Include(a => a.Lesson)
                .Include(a => a.Child)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AttendanceDto>>(attendances);
        }

        public async Task<AttendanceDto?> GetByIdAsync(Guid id)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Lesson)
                .Include(a => a.Child)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
                return null;

            return _mapper.Map<AttendanceDto>(attendance);
        }

        public async Task<AttendanceDto> CreateAsync(AttendanceCreateDto dto)
        {
            var attendance = new Attendance
            {
                Id = Guid.NewGuid(),
                LessonId = dto.LessonId,
                ChildId = dto.ChildId,
                Status = dto.Status,
                Note = dto.Note
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return _mapper.Map<AttendanceDto>(attendance);
        }

        public async Task<AttendanceDto?> UpdateAsync(Guid id, AttendanceUpdateDto dto)
        {
            var attendance = await _context.Attendances.FindAsync(id);

            if (attendance == null)
                return null;

            attendance.Status = dto.Status;
            attendance.Note = dto.Note;

            await _context.SaveChangesAsync();

            return _mapper.Map<AttendanceDto>(attendance);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var attendance = await _context.Attendances.FindAsync(id);

            if (attendance == null)
                return false;

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
