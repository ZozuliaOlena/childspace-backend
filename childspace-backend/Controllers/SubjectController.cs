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
    public class SubjectController : BaseController 
    {
        private readonly ISubjectRepository _repository;

        public SubjectController(ISubjectRepository repository, UserManager<User> userManager)
            : base(userManager)
        {
            _repository = repository;
        }

        [HttpGet]
        [AllowAnonymous] 
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetAll([FromQuery] Guid? centerId)
        {
            var subjects = await _repository.GetAllAsync(centerId);
            return Ok(subjects);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous] 
        public async Task<ActionResult<SubjectDto>> GetById(Guid id)
        {
            var subject = await _repository.GetByIdAsync(id);

            if (subject == null)
                return NotFound();

            return Ok(subject);
        }

        [HttpPost]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<SubjectDto>> Create(SubjectCreateDto dto)
        {
            if (!await CheckCenterPermissionsAsync(dto.CenterId))
                return Forbid();

            var created = await _repository.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<SubjectDto>> Update(Guid id, [FromForm] SubjectUpdateDto dto)
        {
            var existingSubject = await _repository.GetByIdAsync(id);
            if (existingSubject == null) return NotFound();

            if (!await CheckCenterPermissionsAsync(existingSubject.CenterId))
                return Forbid();

            var updated = await _repository.UpdateAsync(id, dto);

            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var existingSubject = await _repository.GetByIdAsync(id);
            if (existingSubject == null) return NotFound();

            if (!await CheckCenterPermissionsAsync(existingSubject.CenterId))
                return Forbid();

            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("my-center")]
        [Authorize(Roles = StaticDetail.Role_Teacher)]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetMyCenterSubjects()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            var subjects = await _repository.GetSubjectsByTeacherAsync(user.Id);

            return Ok(subjects);
        }
    }
}
