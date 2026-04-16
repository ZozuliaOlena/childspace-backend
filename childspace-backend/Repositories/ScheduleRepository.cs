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

        public async Task<IEnumerable<ScheduleDto>> GetAllAsync(Guid? centerId = null)
        {
            var query = _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .Include(s => s.Attendances)
                .AsQueryable();

            if (centerId.HasValue)
            {
                query = query.Where(s => s.Group.CenterId == centerId.Value);
            }

            var schedules = await query.ToListAsync();

            var dtos = _mapper.Map<List<ScheduleDto>>(schedules);

            foreach (var dto in dtos)
            {
                var original = schedules.First(s => s.Id == dto.Id);

                dto.GroupName = original.Group?.Name;
                dto.SubjectName = original.Subject?.Name;

                if (original.Teacher != null)
                {
                    dto.TeacherName = $"{original.Teacher.FirstName} {original.Teacher.LastName}";
                }
            }

            return dtos;
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

            var dto = _mapper.Map<ScheduleDto>(schedule);

            dto.GroupName = schedule.Group?.Name;
            dto.SubjectName = schedule.Subject?.Name;

            if (schedule.Teacher != null)
            {
                dto.TeacherName = $"{schedule.Teacher.FirstName} {schedule.Teacher.LastName}";
            }

            return dto;
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

            schedule.GroupId = dto.GroupId; 
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

        public async Task<IEnumerable<ScheduleDto>> GetByTeacherIdAsync(Guid teacherId)
        {
            var now = DateTime.UtcNow;

            var schedules = await _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .Where(s => s.TeacherId == teacherId && s.EndTime >= now)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            var dtos = _mapper.Map<List<ScheduleDto>>(schedules);
            MapNamesToDtos(schedules, dtos);

            return dtos;
        }

        public async Task<IEnumerable<ScheduleDto>> GetByParentIdAsync(Guid parentId)
        {
            var now = DateTime.UtcNow;

            var childrenIds = await _context.Children
                .Where(c => c.ParentId == parentId)
                .Select(c => c.Id)
                .ToListAsync();

            var groupIds = await _context.GroupChildren
                .Where(gc => childrenIds.Contains(gc.ChildId))
                .Select(gc => gc.GroupId)
                .ToListAsync();

            var schedules = await _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .Where(s => groupIds.Contains(s.GroupId) && s.EndTime >= now)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            var dtos = _mapper.Map<List<ScheduleDto>>(schedules);
            MapNamesToDtos(schedules, dtos);

            return dtos;
        }

        public async Task<IEnumerable<ScheduleDto>> GetByGroupIdAsync(Guid groupId)
        {
            var now = DateTime.UtcNow; 

            var schedules = await _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .Where(s => s.GroupId == groupId && s.EndTime >= now)
                .OrderBy(s => s.StartTime) 
                .ToListAsync();

            var dtos = _mapper.Map<List<ScheduleDto>>(schedules);
            MapNamesToDtos(schedules, dtos);

            return dtos;
        }

        private void MapNamesToDtos(IEnumerable<Schedule> schedules, List<ScheduleDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var original = schedules.First(s => s.Id == dto.Id);

                dto.GroupName = original.Group?.Name;
                dto.SubjectName = original.Subject?.Name;

                if (original.Teacher != null)
                {
                    dto.TeacherName = $"{original.Teacher.FirstName} {original.Teacher.LastName}";
                }
            }
        }
    }
}
