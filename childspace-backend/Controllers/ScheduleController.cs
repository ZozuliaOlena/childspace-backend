using childspace_backend.Data;
using childspace_backend.Models;
using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using childspace_backend.Services;
using childspace_backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleRepository _repository;
        private readonly IFirebaseNotificationService _notificationService;
        private readonly ChildSpaceDbContext _context; 

        public ScheduleController(
            IScheduleRepository repository,
            UserManager<User> userManager,
            IFirebaseNotificationService notificationService,
            ChildSpaceDbContext context)
            : base(userManager)
        {
            _repository = repository;
            _notificationService = notificationService;
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetAll([FromQuery] Guid? centerId)
        {
            Guid? filterCenterId = centerId;

            if (!User.IsInRole(StaticDetail.Role_SuperAdmin))
            {
                var user = await GetCurrentUserAsync();
                filterCenterId = user?.CenterId;

                if (filterCenterId == null) return Forbid();
            }

            var schedules = await _repository.GetAllAsync(filterCenterId);
            return Ok(schedules);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<ScheduleDto>> GetById(Guid id)
        {
            var schedule = await _repository.GetByIdAsync(id);

            if (schedule == null)
                return NotFound();

            return Ok(schedule);
        }

        [HttpPost]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<ScheduleDto>> Create(ScheduleCreateDto dto)
        {
            var created = await _repository.CreateAsync(dto);

            await NotifyAffectedUsers(dto.GroupId, dto.TeacherId,
                "Нове заняття!", "У розкладі з'явилося нове заняття.");

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<ScheduleDto>> Update(Guid id, ScheduleUpdateDto dto)
        {
            var updated = await _repository.UpdateAsync(id, dto);

            if (updated == null)
                return NotFound();

            await NotifyAffectedUsers(dto.GroupId, dto.TeacherId,
                "Розклад оновлено", "Час або деталі заняття були змінені.");

            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var schedule = await _repository.GetByIdAsync(id);
            if (schedule == null) return NotFound();

            var deleted = await _repository.DeleteAsync(id);

            if (!deleted) return NotFound();

            await NotifyAffectedUsers(schedule.GroupId, schedule.TeacherId,
                "Заняття скасовано", "Одне із занять у розкладі було скасовано.");

            return NoContent();
        }

        [HttpGet("my")]
        [Authorize(Roles = $"{StaticDetail.Role_Teacher},{StaticDetail.Role_Parent}")]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetMySchedule()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            if (User.IsInRole(StaticDetail.Role_Teacher))
            {
                var schedules = await _repository.GetByTeacherIdAsync(user.Id);
                return Ok(schedules);
            }

            if (User.IsInRole(StaticDetail.Role_Parent))
            {
                var schedules = await _repository.GetByParentIdAsync(user.Id);
                return Ok(schedules);
            }

            return Forbid();
        }

        [HttpGet("group/{groupId:guid}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetByGroup(Guid groupId)
        {
            var schedules = await _repository.GetByGroupIdAsync(groupId);
            return Ok(schedules);
        }

        private async Task NotifyAffectedUsers(Guid? groupId, Guid? teacherId, string title, string body)
        {
            var tokens = new List<string>();

            if (teacherId.HasValue)
            {
                var teacher = await _userManager.FindByIdAsync(teacherId.Value.ToString());
                if (!string.IsNullOrEmpty(teacher?.FcmToken))
                    tokens.Add(teacher.FcmToken);
            }

            if (groupId.HasValue)
            {
                var parentTokens = await _context.GroupChildren
                    .Where(gc => gc.GroupId == groupId.Value)
                    .Select(gc => gc.Child.Parent.FcmToken)
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToListAsync();

                tokens.AddRange(parentTokens);
            }

            foreach (var token in tokens.Distinct())
            {
                await _notificationService.SendNotificationAsync(token, title, body);
            }
        }
    }
}
