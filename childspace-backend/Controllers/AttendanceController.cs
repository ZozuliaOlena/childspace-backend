using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceRepository _repository;

        public AttendanceController(IAttendanceRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetAll()
        {
            var attendances = await _repository.GetAllAsync();
            return Ok(attendances);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AttendanceDto>> GetById(Guid id)
        {
            var attendance = await _repository.GetByIdAsync(id);

            if (attendance == null)
                return NotFound();

            return Ok(attendance);
        }

        [HttpPost]
        public async Task<ActionResult<AttendanceDto>> Create(AttendanceCreateDto dto)
        {
            var created = await _repository.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<AttendanceDto>> Update(Guid id, AttendanceUpdateDto dto)
        {
            var updated = await _repository.UpdateAsync(id, dto);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
