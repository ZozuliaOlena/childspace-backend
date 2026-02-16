using AutoMapper;
using childspace_backend.Data;
using childspace_backend.Models.DTOs;
using childspace_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace childspace_backend.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ChildSpaceDbContext _context;
        private readonly IMapper _mapper;

        public ScheduleRepository(ChildSpaceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllAsync()
        {
            var schedules = await _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .Include(s => s.Attendances)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<ScheduleDto?> GetByIdAsync(Guid id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
                return null;

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<ScheduleDto> CreateAsync(ScheduleCreateDto dto)
        {
            var schedule = new Schedule
            {
                Id = Guid.NewGuid(),
                GroupId = dto.GroupId,
                TeacherId = dto.TeacherId,
                SubjectId = dto.SubjectId,
                RoomName = dto.RoomName,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<ScheduleDto?> UpdateAsync(Guid id, ScheduleUpdateDto dto)
        {
            var schedule = await _context.Schedules.FindAsync(id);

            if (schedule == null)
                return null;

            schedule.TeacherId = dto.TeacherId;
            schedule.SubjectId = dto.SubjectId;
            schedule.RoomName = dto.RoomName;
            schedule.StartTime = dto.StartTime;
            schedule.EndTime = dto.EndTime;

            await _context.SaveChangesAsync();

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var schedule = await _context.Schedules.FindAsync(id);

            if (schedule == null)
                return false;

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
