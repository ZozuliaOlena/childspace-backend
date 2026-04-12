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
        private readonly ICloudinaryRepository _cloudinaryRepository;

        public SubjectController(
            ISubjectRepository repository,
            ICloudinaryRepository cloudinaryRepository,
            UserManager<User> userManager) : base(userManager)
        {
            _repository = repository;
            _cloudinaryRepository = cloudinaryRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetAll([FromQuery] Guid? centerId)
        {
            Guid? filterCenterId = centerId; 

            if (User.Identity != null && User.Identity.IsAuthenticated && !User.IsInRole(StaticDetail.Role_SuperAdmin))
            {
                var user = await GetCurrentUserAsync();
                if (user != null && user.CenterId != null)
                {
                    filterCenterId = user.CenterId;
                }
            }

            var subjects = await _repository.GetAllAsync(filterCenterId);
            return Ok(subjects);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<SubjectDto>> GetById(Guid id)
        {
            var subject = await _repository.GetByIdAsync(id);
            if (subject == null) return NotFound();
            return Ok(subject);
        }

        [HttpPost]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<SubjectDto>> Create([FromForm] SubjectCreateDto dto)
        {
            if (!await CheckCenterPermissionsAsync(dto.CenterId))
                return Forbid();

            string? photoUrl = null;

            if (dto.Photo != null)
            {
                var uploadResult = await _cloudinaryRepository.UploadAsync(dto.Photo);
                if (uploadResult == null) return BadRequest(new { message = "Помилка завантаження фото" });
                photoUrl = uploadResult.Url;
            }

            var created = await _repository.CreateAsync(dto, photoUrl);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<SubjectDto>> Update(Guid id, [FromForm] SubjectUpdateDto dto)
        {
            var existingSubject = await _repository.GetByIdAsync(id);
            if (existingSubject == null) return NotFound();

            if (!await CheckCenterPermissionsAsync(existingSubject.CenterId))
                return Forbid();

            string? newPhotoUrl = existingSubject.PhotoUrl;

            if (dto.Photo != null)
            {
                var uploadResult = await _cloudinaryRepository.UploadAsync(dto.Photo);
                if (uploadResult == null) return BadRequest(new { message = "Помилка завантаження нового фото" });

                newPhotoUrl = uploadResult.Url;

                var oldPublicId = CloudinaryHelper.ExtractPublicIdFromUrl(existingSubject.PhotoUrl);
                if (!string.IsNullOrEmpty(oldPublicId))
                {
                    try { await _cloudinaryRepository.DeleteAsync(oldPublicId); }
                    catch (Exception ex) { Console.WriteLine($"Failed to delete old photo: {ex.Message}"); }
                }
            }

            var updated = await _repository.UpdateAsync(id, dto, newPhotoUrl);
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
            if (!deleted) return NotFound();

            var publicId = CloudinaryHelper.ExtractPublicIdFromUrl(existingSubject.PhotoUrl);
            if (!string.IsNullOrEmpty(publicId))
            {
                try { await _cloudinaryRepository.DeleteAsync(publicId); }
                catch (Exception ex) { Console.WriteLine($"Failed to delete photo: {ex.Message}"); }
            }

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
