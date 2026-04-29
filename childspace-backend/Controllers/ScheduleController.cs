using childspace_backend.Models;
using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using childspace_backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleRepository _repository;

        public ScheduleController(IScheduleRepository repository, UserManager<User> userManager)
            : base(userManager)
        {
            _repository = repository;
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

            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

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
    }
}
