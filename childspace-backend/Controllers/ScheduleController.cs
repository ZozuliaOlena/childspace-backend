using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using childspace_backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleRepository _repository;

        public ScheduleController(IScheduleRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetAll()
        {
            var schedules = await _repository.GetAllAsync();
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
        [Authorize(Roles = StaticDetail.Role_Teacher)]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetMySchedule()
        {
            var teacherId = Guid.Parse(User.FindFirst("sub")?.Value!);
            var schedules = await _repository.GetByTeacherIdAsync(teacherId);
            return Ok(schedules);
        }

        [HttpGet("children")]
        [Authorize(Roles = StaticDetail.Role_Parent)]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetChildrenSchedule()
        {
            var parentId = Guid.Parse(User.FindFirst("sub")?.Value!);
            var schedules = await _repository.GetByParentIdAsync(parentId);
            return Ok(schedules);
        }
    }
}
