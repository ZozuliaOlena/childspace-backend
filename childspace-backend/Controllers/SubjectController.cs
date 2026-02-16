using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using childspace_backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectRepository _repository;

        public SubjectController(ISubjectRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetAll()
        {
            var subjects = await _repository.GetAllAsync();
            return Ok(subjects);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SubjectDto>> GetById(Guid id)
        {
            var subject = await _repository.GetByIdAsync(id);

            if (subject == null)
                return NotFound();

            return Ok(subject);
        }

        [HttpPost]
        public async Task<ActionResult<SubjectDto>> Create(SubjectCreateDto dto)
        {
            var created = await _repository.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<SubjectDto>> Update(Guid id, SubjectUpdateDto dto)
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

        [HttpGet("my-center")]
        [Authorize(Roles = StaticDetail.Role_Teacher)]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetMyCenterSubjects()
        {
            var userId = User.FindFirst("sub")?.Value;
            if (userId == null) return Unauthorized();

            var subjects = await _repository.GetSubjectsByTeacherAsync(Guid.Parse(userId));

            return Ok(subjects);
        }
    }
}
