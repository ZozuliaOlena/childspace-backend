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
            bool isBusy = await IsTeacherBusyAsync(dto.TeacherId, dto.StartTime, dto.EndTime);
            if (isBusy)
                throw new Exception("Цей вчитель вже має заняття в обраний проміжок часу.");

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

            bool isBusy = await IsTeacherBusyAsync(dto.TeacherId, dto.StartTime, dto.EndTime, id);
            if (isBusy)
                throw new Exception("Цей вчитель вже має інше заняття в обраний проміжок часу.");

            schedule.GroupId = dto.GroupId;
            schedule.TeacherId = dto.TeacherId;
            schedule.SubjectId = dto.SubjectId;
            schedule.RoomName = dto.RoomName;
            schedule.StartTime = dto.StartTime;
            schedule.EndTime = dto.EndTime;

            await _context.SaveChangesAsync();

            return _mapper.Map<ScheduleDto>(schedule);
        }

        private async Task<bool> IsTeacherBusyAsync(Guid? teacherId, DateTime startTime, DateTime endTime, Guid? excludeScheduleId = null)
        {
            if (!teacherId.HasValue)
                return false;

            var query = _context.Schedules.Where(s => s.TeacherId == teacherId.Value);

            if (excludeScheduleId.HasValue)
            {
                query = query.Where(s => s.Id != excludeScheduleId.Value);
            }

            return await query.AnyAsync(s =>
                startTime < s.EndTime && endTime > s.StartTime);
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
            var schedules = await _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .Where(s => s.TeacherId == teacherId)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            var dtos = _mapper.Map<List<ScheduleDto>>(schedules);
            MapNamesToDtos(schedules, dtos);

            return dtos;
        }

        public async Task<IEnumerable<ScheduleDto>> GetByParentIdAsync(Guid parentId)
        {
            var schedules = await _context.Schedules
                .AsNoTracking()
                .Include(s => s.Group)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .Where(s => _context.GroupChildren
                    .Any(gc => gc.GroupId == s.GroupId &&
                               _context.Children.Any(c => c.Id == gc.ChildId && c.ParentId == parentId)))
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            var dtos = _mapper.Map<List<ScheduleDto>>(schedules);

            MapNamesToDtos(schedules, dtos);

            return dtos;
        }

        public async Task<IEnumerable<ScheduleDto>> GetByGroupIdAsync(Guid groupId)
        {
            var schedules = await _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .Where(s => s.GroupId == groupId)
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
